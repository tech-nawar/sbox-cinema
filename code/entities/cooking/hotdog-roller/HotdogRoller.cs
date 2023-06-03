﻿using Editor;
using Sandbox;

namespace Cinema;

[Library("ent_hotdog_roller"), HammerEntity]
[Title("Hotdog Roller"), Category("Gameplay"), Icon("microwave")]
[EditorModel("models/hotdogroller/hotdogroller.vmdl")]
public partial class HotdogRoller : AnimatedEntity, ICinemaUse
{
    [BindComponent] public HotdogRollerKnobs Knobs { get; }
    [BindComponent] public HotdogRollerSwitches Switches { get; }
    [BindComponent] public HotdogRollerRollers Rollers { get; }

    /// <summary>
    /// Sets up the model when spawned by the server
    /// Sets model
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

        SetupModel();
    }
    /// <summary>
    /// Sets up the model when spawned by the server
    /// Sets clientside UI
    /// </summary>
    public override void ClientSpawn()
    {
        base.ClientSpawn();

        SetupUI();
    }
    /// <summary>
    /// Sets up the model when spawned by the server
    /// Sets attached entity components
    /// Adds tags
    /// </summary>
    private void SetupModel()
    {
        Transmit = TransmitType.Always;

        SetModel("models/hotdogroller/hotdogroller.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

        Components.Create<HotdogRollerKnobs>();
        Components.Create<HotdogRollerSwitches>();
        Components.Create<HotdogRollerRollers>();
        
        Tags.Add("interactable");
    }
}