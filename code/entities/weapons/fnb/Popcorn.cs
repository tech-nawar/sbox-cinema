﻿using Sandbox;

namespace Cinema;

public partial class Popcorn : WeaponBase
{
    public override string Name => "Bucket of Popcorn";
    public override string Description => "Tasty popcorn delights";
    public override string Icon => "ui/icons/popcorn.png";
    public override Model WorldModel => Model.Load("models/popcorn_tub/w_popcorn_tub_01.vmdl");
    public override string ViewModelPath => "models/popcorn_tub/v_popcorn_tub_01.vmdl";
    public override float PrimaryFireRate => 0.85f;
    public override int BaseUses => 10;

    public override void Spawn()
    {
        base.Spawn();
    }

    public override void PrimaryFire()
    {
        base.PrimaryFire();

        PlaySound("placeholder_eating");
    }

    public override void SecondaryFire()
    {
        if (Game.IsClient) return;

        using (Prediction.Off())
        {
            var projectile = new Projectile()
            {
                Model = Model.Load("models/popcorn_tub/w_popcorn_tub_01.vmdl"),
            };

            projectile.LaunchFromEntityViewpoint(WeaponHolder);
            
            RemoveFromHolder();
        }
    }

    public override void Reload()
    {
    }
}
