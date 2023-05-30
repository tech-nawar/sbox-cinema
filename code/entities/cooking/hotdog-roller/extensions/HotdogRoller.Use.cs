﻿using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    /// <summary>
    /// Whether this Hotdog Roller is usable or not
    /// </summary>
    /// <param name="user">The player who is using</param>
    /// <returns>If this is useable</returns>
    public virtual bool IsUsable(Entity user)
    {
        return true;
    }

    /// <summary>
    /// Called on the server when the Hotdog Roller is used by a player
    /// </summary>
    /// <param name="user"></param>
    /// <returns>If the player can continue to use the Hotdog Roller</returns>
    public virtual bool OnUse(Entity user)
    {
        if (Game.IsClient) return false;

        TraceResult tr = Trace.Ray(user.AimRay, 2000)
                            .EntitiesOnly()
                            .Ignore(user)
                            .WithoutTags("cookable")
                            .Run();

        if (tr.Hit)
        {
            if (tr.Body is PhysicsBody body)
            {

                TryInteraction(body.GroupName);

            }
        }

        Input.Clear("use"); // Why?

        return false;
    }

    /// <summary>
    /// Called on the server when the Hotdog Roller is stopped being used by a player
    /// </summary>
    /// <param name="user"></param>
    /// <returns>If the player can continue to use the Hotdog Roller</returns>
    public void OnStopUse(Entity user)
    {
        
    }
}
