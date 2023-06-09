﻿using Sandbox;
using Sandbox.UI;

namespace Cinema.UI;

public partial class NameTag : WorldPanel
{
    /// <summary>
    /// Fade distance of name tags, for all players
    /// </summary>
    [ConVar.Replicated("nametag.distance")]
    public static float FadeDistance { get; set; } = 500f;

    public Player Player { get; set; }

    public NameTag(Player player)
    {
        Player = player;
    }

    public override void Tick()
    {
        var IsOutOfRange = Player.Position.Distance(Camera.Position) > FadeDistance;

        if (!Player.IsValid)
        {
            Delete();
            return;
        }

        if (Player.IsFirstPersonMode || IsOutOfRange)
        {
            Title.Style.Opacity = 0;
        }
        else if ((Game.LocalPawn as Player)?.ActiveController is ChairController && Player.ActiveController is ChairController)
        {
            Title.Style.Opacity = 0;
        }
        else
        {
            Title.Style.Opacity = 1;
        }


        Position = Player.GetBoneTransform("head").Position + Vector3.Up * 20;
        Rotation = Rotation.LookAt(Camera.Rotation.Forward * -1.0f, Vector3.Up);
    }
}
