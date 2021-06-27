using FadeBot.Functions;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Threading;

public static class PhotonExtensions
{
    public static string GetUsername(this Player player)
    {
        if (player.CustomProperties.ContainsKey("user"))
            if (player.CustomProperties["user"] is Dictionary<string, object> dict)
                return (string)dict["username"];
        return "No Username";
    }

    public static string GetDisplayName(this Player player)
    {
        if (player.CustomProperties.ContainsKey("user"))
            if (player.CustomProperties["user"] is Dictionary<string, object> dict)
                return (string)dict["displayName"];
        return "No DisplayName";
    }

    public static string GetAvatarStatus(this Player player)
    {
        if (player.CustomProperties["avatarDict"] is Dictionary<string, object> dict)
            return (string)dict["releaseStatus"];
        return "No Status";
    }

    public static string GetAvatarID(this Player player)
    {
        if (player.CustomProperties["avatarDict"] is Dictionary<string, object> dict)
            return (string)dict["id"];
        return "No ID";
    }

    public static string GetAPIUserID(this Player player)
    {
        if (player.CustomProperties.ContainsKey("user"))
            if (player.CustomProperties["user"] is Dictionary<string, object> dict)
                return (string)dict["id"];
        return "";
    }

    public static Player GetPlayer(this int target)
    {
        foreach (var player in Load.photonClients[0].CurrentRoom.Players)
        {
            if (player.Value.ActorNumber == target) return player.Value;
        }
        return null;
    }

    public static void EventSpammer(this int count, int amount, Action action, int? sleep = null)
    {
        for (int ii = 0; ii < count; ii++)
        {
            for (int i = 0; i < amount; i++)
                action();
            if (sleep != null)
                Thread.Sleep(sleep.Value);
            else
                Thread.Sleep(25);
        }
    }
}