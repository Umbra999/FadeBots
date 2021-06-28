using FadeBot.Functions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace FadeBot
{
    internal static class Misc
    {
        public static void AuthCacheMethod()
        {
            List<string> tokens = File.ReadAllLines("Tokens.txt").ToList();
            if (tokens.Count == 0) Logger.LogWarning("No Tokens found -> Skip");
            else
            {
                foreach (var token in tokens)
                {
                    var Split = token.Split(Convert.ToChar(":"));
                    var Auth = Split[0];
                    var UserID = Split[1];
                    string Validation = HttpUtils.ValidateAuthcookie(Auth);
                    if (Validation == null) Logger.LogError($"Failed to validate Token {Auth}");
                    Load.photonClients.Add(new PhotonClient(UserID, Auth, "usw"));
                    Load.photonClients.Add(new PhotonClient(UserID, Auth, "eu"));
                    Load.photonClients.Add(new PhotonClient(UserID, Auth, "jp"));
                }
            }
        }

        public static void ReqAuthMethod()
        {
            string[] auths;
            if (Load.WebsocketMode) auths = File.ReadAllLines("FadeAway\\Photon\\Userpass.txt");
            else auths = File.ReadAllLines("userpass.txt");
            if (auths.Length == 0) Logger.LogWarning("No Userpass found -> Skip");
            else Logger.Log("Found Userpass, assembling....");
            foreach (string line in auths)
            {
                string Cookie = HttpUtils.GetCookie(line);
                var Split = Cookie.Split(Convert.ToChar(":"));
                var Auth = Split[0];
                var UserID = Split[1];
                string Validation = HttpUtils.ValidateAuthcookie(Auth);
                if (Validation == null) Logger.LogError($"Failed to validate Token {Auth}");
                Load.photonClients.Add(new PhotonClient(UserID, Auth, "usw"));
                Load.photonClients.Add(new PhotonClient(UserID, Auth, "eu"));
                Load.photonClients.Add(new PhotonClient(UserID, Auth, "jp"));
                Load.Authcookies.Add(Auth);
                Thread.Sleep(2000);
            }
        }

        public static void ConvertToToken()
        {
            string[] auths = File.ReadAllLines("userpass.txt");
            if (auths.Length == 0) Logger.LogWarning("No Userpass found -> Skip");
            else Logger.Log("Found Userpass, assembling....");
            foreach (string line in auths)
            {
                string Cookie = HttpUtils.GetCookie(line);
                var Split = Cookie.Split(Convert.ToChar(":"));
                var Auth = Split[0];
                var UserID = Split[1];
                string Validation = HttpUtils.ValidateAuthcookie(Auth);
                if (Validation == null) Logger.LogError($"Failed to validate Token {Auth}");
                Load.photonClients.Add(new PhotonClient(UserID, Auth, "usw"));
                Load.photonClients.Add(new PhotonClient(UserID, Auth, "eu"));
                Load.photonClients.Add(new PhotonClient(UserID, Auth, "jp"));
                Load.Authcookies.Add(Auth);
                File.AppendAllText("Tokens.txt", Auth + ":" + UserID + Environment.NewLine);
                Thread.Sleep(2000);
            }
        }

        public static string OnlinePlayers = HttpUtils.HttpGetUsers();

        public static void SendVisitLoop()
        {
            new Thread(() =>
            {
                for (; ; )
                {
                    try
                    {
                        foreach (PhotonClient pc in Load.photonClients)
                        {
                            if (pc.InRoom) HttpUtils.HttpJoinVisit("visits", pc.GetToken(), $"usr_{pc.UserId}", pc.CurrentRoom.Name);
                        }
                    }
                    catch { }
                    Thread.Sleep(30000);
                }
            }).Start();
        }

        public static void JoinRoomAll(string WorldID, bool Instantiate, bool Silent = false)
        {
            foreach (PhotonClient client in Load.photonClients)
            {
                if (client.IsConnectedAndReady)
                {
                    new Thread(() =>
                    {
                        if (client.InRoom)
                        {
                            client.LeaveRoom();
                            Thread.Sleep(3000);
                        }
                        if (Instantiate) 
                        {
                            PhotonClient.ShouldInstantiate = true;
                            if (Silent) PhotonClient.InstantiateSilent = true;
                            else PhotonClient.InstantiateSilent = false;
                        }
                        else PhotonClient.ShouldInstantiate = false;

                        string region = "usw";
                        if (WorldID.Contains("~region(jp)")) region = "jp";
                        else if (WorldID.Contains("~region(eu)")) region = "eu";
                        if (WorldID.Contains("wrld_") && client.CloudRegion.Contains(region)) client.JoinRoom(WorldID);
                    }).Start();
                }
            }
        }

        public static string FetchRoomToken(string worldId, string token, string UserID)
        {
            var response = HttpUtils.HttpGetRoomtoken(worldId, token);
            HttpUtils.HttpJoinVisit("joins", token, worldId, UserID);
            if (response != null) return JsonConvert.DeserializeObject<room>(response).token;
            return null;
        }

        public static room FetchWorld(string worldId, string token)
        {
            string json = HttpUtils.HttpGetWorld(worldId, token);
            if (json != null) return JsonConvert.DeserializeObject<room>(json);
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

        public static instance[] GetInstances(room world)
        {
            try
            {
                List<instance> instances = new List<instance>();
                if (world.instances == null) return null;
                foreach (var instance in world.instances)
                {
                    instance Current = new instance
                    {
                        count = Convert.ToInt32(instance[1].ToString()),
                        id = instance[0].ToString()
                    };
                    instances.Add(Current);
                }
                return instances.ToArray();
            }
            catch { }
            return null;
        }

        public static string GetToken(this PhotonClient Client)
        {
            string[] Token = Client.AuthValues.AuthGetParameters.Split(Convert.ToChar("="), Convert.ToChar("&"));
            return Token[1];
        }

        public static string GetUserRank(JEnumerable<JToken> UserTags)
        {
            string Rank;
            if (UserTags.Contains("system_legend") && UserTags.Contains("system_trust_legend") && UserTags.Contains("system_trust_trusted")) Rank = "Legendary";
            else if (UserTags.Contains("system_trust_legend") && UserTags.Contains("system_trust_trusted")) Rank = "Veteran";
            else if (UserTags.Contains("system_trust_veteran") && UserTags.Contains("system_trust_trusted")) Rank = "Trusted";
            else if (UserTags.Contains("system_trust_trusted") && UserTags.Contains("system_trust_known")) Rank = "Known";
            else if (UserTags.Contains("system_trust_basic") && UserTags.Contains("system_trust_known")) Rank = "User";
            else if (UserTags.Contains("system_trust_basic")) Rank = "New";
            else Rank = "Visitor";

            return Rank;
        }
    }

    public class user
    {
        public string id { get; set; }
        public string username { get; set; }
        public string displayName { get; set; }
    }

    public class instance
    {
        public string id { get; set; }
        public int count { get; set; }
    }

    public class auth
    {
        public bool ok { get; set; }
        public string token { get; set; }
    }

    public class PutObject
    {
        public string ID { get; set; }
        public string Value { get; set; }
    }

    public class room
    {
        public int capacity { get; set; }
        public int version { get; set; }
        public string token { get; set; }
        public int publicOccupants { get; set; }
        public object[][] instances { get; set; }
    }

    public class Serialization
    {
        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null) return null;
            BinaryFormatter bf = new BinaryFormatter();
            using MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null) return default;
            BinaryFormatter bf = new BinaryFormatter();
            using MemoryStream ms = new MemoryStream(data);
            object obj = bf.Deserialize(ms);
            return (T)obj;
        }
    }
}