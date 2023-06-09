﻿using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace Cinema.UI;

public partial class Scoreboard : Panel, IMenuScreen
{
    public static Scoreboard Instance { get; set; }
    public string Name => Game.Server.ServerTitle;
    public bool IsOpen { get; protected set; }
    public string VisibleClass => IsOpen ? "visible" : "";
    public IReadOnlyCollection<IClient> Clients => Game.Clients;

    public Scoreboard()
    {
        Instance = this;
    }
    public bool Open()
    {
        IsOpen = true;

        return true;
    }
    public void Close()
    {
        IsOpen = false;
    }
    protected override int BuildHash()
    {
        var clientHash = 11;

        foreach(var client in Clients)
        {
            clientHash = clientHash * 31 + HashCode.Combine(client.Name, client.Ping);
        }

        return HashCode.Combine(IsOpen, Clients.Count, clientHash);
    }
}
