﻿using Editor;
using Sandbox;

namespace Cinema;

[Library("ent_projector"), HammerEntity]
[EditorModel("models/editor/ortho", "rgb(0, 255, 192)", "rgb(255, 64, 64)")]
[Title("Projector"), Category("Gameplay"), Icon("monitor")]
[SupportsSolid]
public partial class ProjectorEntity : Entity
{
    [Property(Title = "Projector Name")]
    public string ProjectorName { get; set; } = "Projector";

    [Net, Property(Title = "Projection Resolution (Pixels)")]
    public Vector2 ProjectionResolution { get; set; }

    [Net, Property(Title = "Projection Size (Units)")]
    public Vector2 ProjectionSize { get; set; }

    public Projection ProjectionImage { get; set; }
    public WebMediaSource MediaSrc { get; set; }

    public MediaController Controller { get; set; }

    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        if (ProjectionResolution == default)
        {
            ProjectionResolution = new Vector2(1024);
        }
        if (ProjectionSize == default)
        {
            ProjectionSize = new Vector2(240, 135);
        }

        Controller = new MediaController()
        {
            Projector = this
        };
        Controller.Parent = this;
    }
    public override void ClientSpawn()
    {
        base.ClientSpawn();

        ProjectionImage = new Projection(this, MediaSrc);
    }

    public static void DrawGizmos(EditorContext context)
    {
        if (!context.IsSelected)
        {
            return;
        }

        var projectionSizeProp = context.Target.GetProperty("ProjectionSize");
        var projectionSize = projectionSizeProp.As.Vector3;

        var length = 3000f;
        var mins = new Vector3(0, -(projectionSize.x / 2), -(projectionSize.y / 2));
        var maxs = new Vector3(length, projectionSize.x / 2, projectionSize.y / 2);
        var bbox = new BBox(mins, maxs);
        Gizmo.Draw.LineBBox(bbox);
    }
}

