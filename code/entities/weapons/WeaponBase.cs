﻿using Sandbox;
using System.Linq;

namespace Cinema;

public partial class WeaponBase : Carriable
{
    [Net]
    public string WeaponName { get; protected set; } = "Generic Weapon";
    [Net]
    public string Description { get; protected set; } = "A generic weapon's description";
    [Net]
    public string Icon { get; protected set; } = "";
    [Net]
    public Model WorldModel { get; protected set; } = null;
    public virtual float DeployingTime => 0.25f;
    public virtual float PrimaryFireRate => 0.1f;
    public virtual float SecondaryFireRate => 0.2f;
    public virtual float ReloadingTime => 0.5f;
    public virtual bool AutoPrimary => false;
    public virtual bool AutoSecondary => false;
    public Player WeaponHolder => Owner as Player;
    [Net] public bool IsReloading { get; set; } = false;
    [Net, Predicted] public TimeUntil DeployTime { get; set; }
    [Net, Predicted] public TimeUntil ReloadTime { get; set; }
    [Net, Predicted] public TimeSince LastPrimaryFire { get; set; }
    [Net, Predicted] public TimeSince LastSecondaryFire { get; set; }

    //How many uses (when spawned) does this weapon have before expiring/dropping
    public virtual int BaseUses => 1;

    public int UsesRemaining;

    public override void Spawn()
    {
        base.Spawn();

        Model = WorldModel;
        UsesRemaining = BaseUses;
    }

    public virtual void LoadResourceData(HandheldResource resource)
    {
        WeaponName = resource.ItemName;
        Description = resource.Description;
        Icon = resource.Icon;
        WorldModel = Model.Load(resource.WorldModel);
        ViewModelPath = resource.ViewModel;
    }

    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);

        if (CanHolster())
        {
            Holster();
            return;
        }

        //If the holder is reloading, wait until the reload is finished
        if (IsReloading)
        {
            if (ReloadTime <= 0.0f)
            {
                FinishReload();
            }

            return;
        }

        //Weapon holder (the player) may have become invalid
        if (!WeaponHolder.IsValid())
            return;

        if (CanReload())
        {
            Reload();
        }

        //Reloading may have changed the weapon holder
        if (!WeaponHolder.IsValid())
            return;

        if (CanFirePrimary())
        {
            using (LagCompensation())
            {
                PrimaryFire();
            }
        }

        //Similar to above but after primary fire
        if (!WeaponHolder.IsValid())
            return;

        if (CanFireSecondary())
        {
            using (LagCompensation())
            {
                SecondaryFire();
            }
        }
    }

    //Can the holder primary fire
    public virtual bool CanFirePrimary()
    {
        if (DeployTime > 0.0f) return false;

        if (UsesRemaining <= 0) return false;

        return LastPrimaryFire >= PrimaryFireRate &&
            (AutoPrimary ? Input.Down("attack1") : Input.Pressed("attack1"));
    }

    //Can the holder secondary fire
    public virtual bool CanFireSecondary()
    {
        if (DeployTime > 0.0f) return false;

        return LastPrimaryFire >= PrimaryFireRate &&
            (AutoPrimary ? Input.Down("attack2") : Input.Pressed("attack2"));
    }

    //Can the holder reload (if any)
    public virtual bool CanReload()
    {
        if (DeployTime > 0.0f) return false;

        return !IsReloading && Input.Pressed("reload");
    }

    public virtual bool CanHolster()
    {
        return Input.Pressed("holster");
    }

    //Primary fire
    public virtual void PrimaryFire()
    {
        UsesRemaining--;

        LastPrimaryFire = 0;
        LastSecondaryFire = 0;
    }

    //Secondary fire
    public virtual void SecondaryFire()
    {
        LastPrimaryFire = 0;
        LastSecondaryFire = 0;
    }

    //Reloading, this can be left empty if not required
    public virtual void Reload()
    {
        IsReloading = true;
        ReloadTime = ReloadingTime;
    }

    public virtual void Holster()
    {
        WeaponHolder.ActiveChild = null;
    }

    //Finishes reloading
    public virtual void FinishReload()
    {
        IsReloading = false;
    }

    //Equips the weapon, does any setup when deployed
    public override void ActiveStart(Entity ent)
    {
        base.ActiveStart(ent);
        DeployTime = DeployingTime;
    }

    //Holsters the weapon
    public override void ActiveEnd(Entity ent, bool dropped)
    {
        base.ActiveEnd(ent, dropped);
        IsReloading = false;
    }

    public override void CreateViewModel()
    {
        Game.AssertClient();

        if (string.IsNullOrEmpty(ViewModelPath))
            return;

        ViewModelEntity = new BaseViewModel();
        ViewModelEntity.Position = Camera.Position;
        ViewModelEntity.Owner = Owner;
        ViewModelEntity.EnableViewmodelRendering = true;
        ViewModelEntity.SetModel(ViewModelPath);

        BaseViewModel arms = new BaseViewModel();
        arms.Position = Camera.Position;
        arms.Owner = Owner;
        arms.EnableViewmodelRendering = true;
        arms.SetModel("models/first_person_citizen/first_person_citizen.vmdl");
        arms.SetParent(ViewModelEntity, true);
    }

    public void RemoveFromHolder()
    {
        if (!WeaponHolder.IsValid())
            return;

        WeaponHolder.Inventory.Remove(Item);
        // Get the next weapon of the same type if we have one
        var nextWeapon = WeaponHolder
            .Weapons
            .FirstOrDefault(w => w.WeaponName == WeaponName);
        nextWeapon ??= WeaponHolder.GetBestWeapon();
        WeaponHolder.ActiveChild = nextWeapon;
    }
}

