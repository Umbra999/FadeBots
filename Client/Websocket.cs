using FadeBot;
using FadeBot.Functions;
using System.Diagnostics;
using System.Threading;
using WebSocketSharp;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace FadeBots.Client
{
    internal class Websocket
    {
        public static WebSocket WSClient;

        public static void Initialize()
        {
            WSClient = new WebSocket("ws://localhost:6666/Bot");
            WSClient.Log.Level = LogLevel.Fatal;
            WSClient.OnMessage += OnServerMessage;
            WSClient.OnError += OnError;
            WSClient.OnClose += OnClose;
            WSClient.Connect();
            SendMessage($"ConsoleLog/Bot PhotonBot connected to the server");
        }

        public static void SendMessage(string Message)
        {
            WSClient.Send(Message);
        }

        public static void OnClose(object Sender, CloseEventArgs Close)
        {
            SendMessage($"ConsoleLog/Bot PhotonBot closed Websocket");
            WSClient.Connect();
        }

        public static void OnError(object Sender, ErrorEventArgs Error)
        {
            SendMessage($"ConsoleLog/Bot PhotonBot had errors");
        }

        public static void OnServerMessage(object Sender, MessageEventArgs Message)
        {
            var Split = Message.Data.Split('/');
            CommandsHandler(Split);
        }

        public static void CommandsHandler(string[] FullCommand)
        {
            var Command = "";
            var Data = "";
            var SecondData = "";
            if (FullCommand.Length > 0) Command = FullCommand[0];
            if (FullCommand.Length > 1) Data = FullCommand[1];
            if (FullCommand.Length > 2) SecondData = FullCommand[2];

            switch (Command)
            {
                case "Shutdown":
                    Events.OnShutdown();
                    break;

                case "CheckInstance":
                    Events.CheckInstance(Data);
                    break;

                case "JoinRoom":
                    Misc.JoinRoomAll(Data, true);
                    break;

                case "Nuke":
                    Exploits.Event6Exploit();
                    break;

                case "ChangeAvatar":
                    foreach (PhotonClient pc in Load.photonClients)
                    {
                        if (pc.CloudRegion.Contains("usw")) HttpUtils.SetAvatar(pc.GetToken(), Data);
                        Exploits.ReloadAvatar(pc);
                    }
                    break;
                case "Friend":
                    foreach (PhotonClient pc in Load.photonClients)
                    {
                        if (pc.CloudRegion.Contains("usw"))
                        {
                            HttpUtils.FriendRequest(pc.GetToken(), Data);
                            Thread.Sleep(4500);
                        }
                    }
                    break;
            }
        }
    }

    public class Events
    {
        public static void OnShutdown()
        {
            foreach (PhotonClient pc in Load.photonClients)
            {
                if (pc.InRoom) pc.LeaveRoom();
            }
            Thread.Sleep(1500);
            Process.GetCurrentProcess().Kill();
        }

        public static void CheckInstance(string Data)
        {
            PhotonClient Bot = Load.photonClients.Find(pc => pc.IsConnectedAndReady && pc.CloudRegion.Contains("usw"));
            PhotonClient.ShouldInstantiate = false;
            PhotonClient.ShouldLog = true;
            if (Data.Contains("~region(jp)")) Bot = Load.photonClients.Find(pc => pc.IsConnectedAndReady && pc.CloudRegion.Contains("jp"));
            else if (Data.Contains("~region(eu)")) Bot = Load.photonClients.Find(pc => pc.IsConnectedAndReady && pc.CloudRegion.Contains("eu"));
            if (Data.Contains("wrld_")) Bot.JoinRoom(Data);
        }
    }
}