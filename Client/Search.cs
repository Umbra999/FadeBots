using FadeBot;
using FadeBot.Functions;
using Photon.Realtime;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading;

namespace FadeBots.Client
{
    internal class Search
    {
        public static bool ShouldCheckRoom = false;
        public static string UsersToSearch;
        public static string ModeratorToSearch;
        public static string StreamerToSearch;
        public static string CreatorToSearch;

        public static void SearchWorld()
        {
            if (UsersToSearch == null || ModeratorToSearch == null || StreamerToSearch == null || CreatorToSearch == null) DownloadSearchlist();
            ShouldCheckRoom = true;
            string[] Worlds = File.ReadAllLines("Worlds.txt");

            PhotonClient USWBot = Load.photonClients.Find(pc => pc.IsConnectedAndReady && pc.CloudRegion.Contains("usw"));
            PhotonClient EUBot = Load.photonClients.Find(pc => pc.IsConnectedAndReady && pc.CloudRegion.Contains("eu"));
            PhotonClient JPBot = Load.photonClients.Find(pc => pc.IsConnectedAndReady && pc.CloudRegion.Contains("jp"));

            string Token = USWBot.GetToken();
            foreach (string World in Worlds)
            {
                room FetchedRoom = Misc.FetchWorld(World, Token);
                instance[] Instances = Misc.GetInstances(FetchedRoom);
                if (Instances != null && FetchedRoom.publicOccupants > 1)
                {
                    foreach (instance Instance in Instances)
                    {
                        if (Instance.count > 1)
                        {
                            string FullWorldID = $"{World}:{Instance.id}";
                            PhotonClient.ShouldInstantiate = false;
                            PhotonClient Bot = USWBot;
                            if (FullWorldID.Contains("~region(jp)")) Bot = JPBot;
                            else if (FullWorldID.Contains("~region(eu)")) Bot = EUBot;
                            Bot.JoinRoom(FullWorldID, FetchedRoom.capacity);
                            while (Bot.InRoom) Thread.Sleep(500);
                            Thread.Sleep(12300);
                        }
                    }
                }
                else Thread.Sleep(5000);
            }
            SearchWorld();
        }

        public static void DownloadSearchlist()
        {
            using WebClient httpClient = new WebClient();
            httpClient.Proxy = null;
            httpClient.Headers.Add("FadeKey", "FadeAway-Still-Top187");

            UsersToSearch = httpClient.DownloadString("https://fadeaway.fadeclient.workers.dev/SearchbotUsers");
            StreamerToSearch = httpClient.DownloadString("https://fadeaway.fadeclient.workers.dev/SearchbotStreamer");
            CreatorToSearch = httpClient.DownloadString("https://fadeaway.fadeclient.workers.dev/SearchbotCreator");
            ModeratorToSearch = httpClient.DownloadString("https://fadeaway.fadeclient.workers.dev/SearchbotModerator");
        }

        public static bool CheckSearchbotUser(string Id)
        {
            return UsersToSearch.Contains(Id);
        }

        public static bool CheckSearchbotCreator(string Id)
        {
            return CreatorToSearch.Contains(Id);
        }

        public static bool CheckSearchbotStreamer(string Id)
        {
            return StreamerToSearch.Contains(Id);
        }

        public static bool CheckSearchbotModerator(string Id)
        {
            return ModeratorToSearch.Contains(Id);
        }

        public static void SendWebHook(string URL, string MSG)
        {
            NameValueCollection pairs = new NameValueCollection()
            {
                { "content", MSG }
            };
            byte[] numArray;
            using WebClient webClient = new WebClient();
            numArray = webClient.UploadValues(URL, pairs);
        }

        public static void CheckInstanceUsers(string Room, Player Player)
        {
            string userid = Player.GetAPIUserID();
            string name = Player.GetDisplayName();
            if (CheckSearchbotUser(userid))
            {
                var UserFound = $" > **[--User Found--]** \n> **Player:** {name}  [{userid}]  \n> **World:** {Room}";
                SendWebHook("https://discord.com/api/webhooks/849316338807865365/A4X0SBFcD7EOPjgnDIi3Cz-PK-lRhJbt_yNzfoP7Xld5w0K5IgiGmW4NQOkoag5pNJQp", UserFound);
            }
            else if (CheckSearchbotCreator(userid))
            {
                var UserFound = $" > **[--Avatar Creator Found--]** \n> **Player:** {name}  [{userid}]  \n> **World:** {Room}";
                SendWebHook("https://discord.com/api/webhooks/849316438770319366/LW2iN1mO0I7U76juzDcAmV_eTqFg3tq8jIwDGcDdm0ZoqRceixBju6hlUtVm0J_WNPUO", UserFound);
            }
            else if (CheckSearchbotModerator(userid))
            {
                var UserFound = $" > **[--Moderator Found--]** \n> **Player:** {name}  [{userid}]  \n> **World:** {Room}";
                SendWebHook("https://discord.com/api/webhooks/849316188823617546/MaUK54dNSZ7k_1SCuS2nypisqlGSHAXNYugBXwrzMbPO_RqLyCeqKk-OgsZcAY6ZMPgb", UserFound);
            }
            else if (CheckSearchbotStreamer(userid))
            {
                var UserFound = $" > **[--Streamer Found--]** \n> **Player:** {name}  [{userid}]  \n> **World:** {Room}";
                SendWebHook("https://discord.com/api/webhooks/849316073337126932/crlcmlAJb7WHvhXS661SbyuFJbDZ2YAk0wE0fVA-Rd6sDpgZ-8rqDCXuIiigL7ZzDFyL", UserFound);
            }
        }
    }
}