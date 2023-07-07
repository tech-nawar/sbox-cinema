using System.Linq;
using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using CinemaTeam.Plugins.Video;

namespace Cinema.UI;

public partial class MovieQueue : Panel, IMenuScreen
{
    public static MovieQueue Instance { get; set; }

    public string Name => "Movie Queue";
    public bool IsOpen { get; protected set; }

    public MovieQueue()
    {
        Instance = this;
    }

    public Panel Thumbnail { get; set; }

    public string VisibleClass => IsOpen ? "visible" : "";
    public Panel MediaProviderHeaderContainer { get; set; }

    public MediaController Controller { get; set; } = null;

    public IList<Media> Queue => /* Controller?.Queue.OrderBy(m => m.ListScore).ToList() ?? */ new List<Media>();

    public MediaRequest NowPlaying => Controller?.CurrentMedia;

    public TimeSince TimeSinceStartedPlaying => Controller?.StartedPlaying ?? 0;

    public string NowPlayingTimeString => NowPlaying == null ? "0:00 / 0:00" : $"{TimeSpan.FromSeconds(TimeSinceStartedPlaying.Relative):hh\\:mm\\:ss} / {TimeSpan.FromSeconds(NowPlaying.GenericInfo.Duration):hh\\:mm\\:ss}";

    public TextEntry MovieIDEntry { get; set; }

    public bool Open()
    {
        if (Game.LocalPawn is not Player ply)
            return false;

        var currentTheaterZone = ply.GetCurrentTheaterZone();

        // If we're not in a theater zone, don't open the queue.
        if (currentTheaterZone == null)
            return false;

        Controller = currentTheaterZone.MediaController;
        IsOpen = true;
        return true;
    }

    public void Close()
    {
        IsOpen = false;
        Controller = null;
    }

    public void SetVideoProvider(IMediaProvider provider)
    {
        if (provider is IMediaSelector selector)
        {
            MediaProviderHeaderContainer.DeleteChildren(true);
            var header = selector.HeaderPanel;
            MediaProviderHeaderContainer.AddChild(header);
            var providerId = VideoProviderManager.Instance.GetKey(provider);
            header.RequestMedia += (s, e) => OnQueue(providerId, e.Query);
        }
    }

    protected void OnQueue(int providerId, string query)
    {
        var zoneId = Controller.Zone.NetworkIdent;
        var clientId = Game.LocalClient.NetworkIdent;
        MediaController.PlayMedia(zoneId, clientId, providerId, query);
    }

    protected static bool CanRemoveMedia(MediaRequest media)
    {
        return false;
        // return media.CanRemove(Game.LocalClient);
    }

    protected static bool IsPlayer(MediaRequest media)
    {
        return media.Requestor == Game.LocalClient;
    }

    protected static bool CanVoteFor(MediaRequest media, bool upvote)
    {
        return false;
        // return upvote ? media.CanUpVote(Game.LocalClient) : media.CanDownVote(Game.LocalClient);
    }

    protected static bool CanGiveLike(MediaRequest media, bool like)
    {
        return false;
        // return like ? media.CanLike(Game.LocalClient) : media.CanDislike(Game.LocalClient);
    }

    protected void OnRemove(MediaRequest media)
    {
        //MediaController.RemoveMedia(Controller.NetworkIdent, Controller.Queue.IndexOf(media));
    }

    protected void OnVote(MediaRequest media, bool voteFor)
    {
        //if (voteFor ? !media.CanUpVote(Game.LocalClient) : !media.CanDownVote(Game.LocalClient)) return;
        //MediaController.VoteForMedia(Controller.NetworkIdent, media.Nonce, voteFor);
    }

    protected void OnLike(MediaRequest media, bool like)
    {
        //if (like ? !media.CanLike(Game.LocalClient) : !media.CanDislike(Game.LocalClient)) return;
        //MediaController.GiveMediaLike(Controller.NetworkIdent, like);
    }

    protected override int BuildHash()
    {
        var queueHash = 11;

        if (Queue != null)
        {
            foreach (var media in Queue)
            {
                queueHash = queueHash * 31 + media.GetHashCode();
            }
        }

        var activeMediaProviderPanel = MediaProviderHeaderContainer.Children.FirstOrDefault();

        return HashCode.Combine(activeMediaProviderPanel, IsOpen, Queue.Count, NowPlaying, queueHash, NowPlayingTimeString);
    }

    public override void Tick()
    {
        Thumbnail?.Style.SetBackgroundImage(NowPlaying.GenericInfo.Thumbnail);
    }

    protected void OnClose()
    {
        (Game.LocalPawn as Player).CloseMenu(this);
    }
}
