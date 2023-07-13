﻿using CinemaTeam.Plugins.Video;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.UI;

public partial class MediaQueueItem : Panel
{
    public MediaQueue.ScoredItem ScoredItem { get; set; }
    public MediaInfo MediaInfo => ScoredItem?.Item?.GenericInfo;
    public MediaQueue Queue { get; set; }

    protected override int BuildHash()
    {
        var hashCode = 0;
        foreach (var vote in ScoredItem.PriorityVotes)
        {
            var voteHash = HashCode.Combine(vote.Key.GetHashCode(), vote.Value.GetHashCode());
            hashCode = HashCode.Combine(hashCode, voteHash);
        }
        return hashCode;
    }

    public void OnVote(bool isUpvote)
    {

    }

    public void OnRemove()
    {

    }
}