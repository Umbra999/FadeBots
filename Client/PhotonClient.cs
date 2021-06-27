using ExitGames.Client.Photon;
using FadeBot.Functions;
using FadeBots.Client;
using Newtonsoft.Json;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace FadeBot
{
    public class PhotonClient : LoadBalancingClient, IConnectionCallbacks, IInRoomCallbacks, ILobbyCallbacks, IMatchmakingCallbacks, IPhotonPeerListener
    {
        private readonly Thread photonThread;
        private readonly Hashtable SendInstantiateEvHashtable = new Hashtable();
        private RaiseEventOptions SendInstantiateRaiseEventOptions = new RaiseEventOptions();
        private PhotonClient instance;

        public PhotonClient(string userid, string auth, string region)
        {
            instance = this;
            AppId = "bf0942f7-9935-4192-b359-f092fa85bef1";
            AppVersion = VRChatLog.FetchAppVersion();
            //AppVersion = "Release_2018_server_1106_2.5";
            Logger.Log($"[PhotonClient] Connected to Server {AppVersion}");
            NameServerHost = "ns.exitgames.com";
            photonThread = new Thread(PhotonLoop) { IsBackground = true };
            photonThread.Start();
            CustomTypes.Register(instance);
            AuthValues = new AuthenticationValues
            {
                AuthType = CustomAuthenticationType.Custom,
            };
            AuthValues.AddAuthParameter("token", auth);
            AuthValues.AddAuthParameter("user", userid);
            AuthValues.AddAuthParameter("hwid", HttpUtils.HWID);
            AuthValues.AddAuthParameter("platform", "android");
            AddCallbackTarget(this);

            if (!ConnectToRegionMaster(region)) Logger.LogError($"Failed to connect to Photon {region}");
        }

        public static string USpeakTarget = "";

        public override void OnEvent(EventData eventData)
        {
            base.OnEvent(eventData);
            //Logger.Log(eventData.Code);
            switch (eventData.Code)
            {
                case 1:
                    if (PhotonExtensions.GetPlayer(eventData.Sender).GetAPIUserID() == USpeakTarget)
                    {
                        if (InRoom)
                        {
                            OpRaiseEvent(1, eventData.CustomData, new RaiseEventOptions()
                            {
                                Receivers = ReceiverGroup.Others,
                            }, SendOptions.SendUnreliable);
                        }
                    }
                    break;

                case 33:
                    var dict = (Dictionary<byte, object>)eventData.CustomData;
                    object outvalue;
                    if (dict.TryGetValue(3, out outvalue))
                    {
                        if (outvalue.ToString() != "System.String[]")
                        {
                            Logger.Log("Found KickID");
                            Exploits.ForcekickID = outvalue.ToString();
                        }
                    }
                    break;

                case 6:
                    var actorNr = BitConverter.ToInt32((byte[])eventData.CustomData, 5);
                    //Logger.Log($"Received RPC from Actor:{actorNr}");
                    break;

                case 2:
                    Logger.Log($"{instance.UserId} got disconnected from Photon Server");
                    break;
            }
        }

        public static bool OpRaiseLog = false;

        public override bool OpRaiseEvent(byte eventCode, object customEventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            base.OpRaiseEvent(eventCode, customEventContent, raiseEventOptions, sendOptions);
            if (OpRaiseLog)
            {
                Logger.LogWarning($"===OPRAISE {eventCode}===");
                Logger.LogWarning($"TYPE:{customEventContent}");
                Logger.LogWarning($"DATA:{JsonConvert.SerializeObject(customEventContent)}");
                Logger.LogWarning($"RECEIVER:{raiseEventOptions.Receivers}");
                Logger.LogWarning($"TARGET ACTORS:{raiseEventOptions.TargetActors}");
                Logger.LogWarning($"CACHING:{raiseEventOptions.CachingOption}");
                Logger.LogWarning($"INTERESTS:{raiseEventOptions.InterestGroup}");
                Logger.LogWarning($"CHANNEL:{raiseEventOptions.SequenceChannel}");
            }
            return true;
        }

        public static bool ResponseLog = false;

        public override void OnOperationResponse(OperationResponse operationResponse)
        {
            base.OnOperationResponse(operationResponse);
            if (ResponseLog)
            {
                Logger.LogWarning($"===OPRESPONSE {operationResponse.OperationCode}===");
                Logger.LogWarning($"RETURNCODE: {operationResponse.ReturnCode}");
                Logger.LogWarning($"MESSAGE: {operationResponse.DebugMessage}");
                Logger.LogWarning($"PARAMETERS: {operationResponse.Parameters}");
                Logger.LogWarning($"DATA: {operationResponse.ToStringFull()}");
            }
        }

        public override void OnMessage(object message)
        {
            base.OnMessage(message);
            //Logger.Log(message);
        }

        private void PhotonLoop()
        {
            for (; ; )
            {
                Service();
                Console.Title = $"FadeBOT - {Load.photonClients.Count / 3} Bots - {Misc.OnlinePlayers} Players";
            }
        }

        public static void UpdateClient()
        {
            for (; ; )
            {
                Logger.LogImportant("");
                Logger.LogImportant("------------------------------");
                Logger.LogImportant("j [worldid] = Join world by ID");
                Logger.LogImportant("s [worldid] = Silent join world by ID");
                Logger.LogImportant("c [worldid] = Connect the Bots by ID");
                Logger.LogImportant("g [worldid] = Check the Instance by ID");
                Logger.LogImportant("l  = Leave the Lobby");
                Logger.LogImportant("sb = Start the Searchbot");
                Logger.LogImportant("");
                Logger.LogImportant("r [userid]  = repeat the Targets voice");
                Logger.LogImportant("v [actornumber]/[all]  = Forcekick the Player");
                Logger.LogImportant("w  = Crash the Lobby with Event 6");
                Logger.LogImportant("d  = Desync the Lobby with Event 210");
                Logger.LogImportant("p [filepath]  = Play Audio trough USpeak");
                Logger.LogImportant("a [avatarid]  = Change the Avatar");
                Logger.LogImportant("f [userid]  = Send a friend request");
                Logger.LogImportant("b [userid]  = Block the User");
                Logger.LogImportant("ub [userid]  = Unblock the User");
                Logger.LogImportant("derank [userid] = Massblock the User");
                Logger.LogImportant("------------------------------");
                Logger.LogImportant("");
                string input = Console.ReadLine();
                var InputStart = input.Split(Convert.ToChar(" "))[0];
                switch (InputStart)
                {
                    case "j":
                        Misc.JoinRoomAll(input.Substring(2), true);
                        break;

                    case "s":
                        Misc.JoinRoomAll(input.Substring(2), true, true);
                        break;

                    case "c":
                        Misc.JoinRoomAll(input.Substring(2), false);
                        break;

                    case "r":
                        USpeakTarget = input.Substring(2);
                        break;

                    case "w":
                        Exploits.Event6Exploit();
                        break;

                    case "d":
                        Exploits.Event210DC();
                        break;

                    case "l":
                        foreach (PhotonClient pc in Load.photonClients)
                        {
                            if (pc.InRoom) pc.LeaveRoom();
                        }
                        break;

                    case "v":
                        if (input.Substring(2) == "all") Exploits.ForceKickAll();
                        else if (input.Substring(2).Contains("usr_")) Exploits.ForceKick(input.Substring(2));
                        break;

                    case "p":
                        //Uspeak Here
                        break;

                    case "a":
                        foreach (PhotonClient pc in Load.photonClients)
                        {
                            if (pc.CloudRegion.Contains("usw")) HttpUtils.SetAvatar(pc.GetToken(), input.Substring(2));
                            Exploits.ReloadAvatar(pc);
                        }
                        break;

                    case "f":
                        foreach (PhotonClient pc in Load.photonClients)
                        {
                            if (pc.CloudRegion.Contains("usw"))
                            {
                                HttpUtils.FriendRequest(pc.GetToken(), input.Substring(2));
                                Thread.Sleep(4500);
                            }
                        }
                        break;

                    case "b":
                        Exploits.Moderate(input.Substring(2), true);
                        break;

                    case "ub":
                        Exploits.Moderate(input.Substring(2), false);
                        break;

                    case "sb":
                        Search.SearchWorld();
                        break;

                    case "derank":
                        Logger.LogImportant("WorldID:");
                        string World = Console.ReadLine();
                        Exploits.Derank(input.Substring(2), World);
                        break;

                    case "g":
                        PhotonClient Bot = Load.photonClients.Find(pc => pc.IsConnectedAndReady && pc.CloudRegion.Contains("usw"));
                        ShouldInstantiate = false;
                        ShouldLog = true;
                        string worldPath = input.Substring(2);
                        if (worldPath.Contains("~region(jp)")) Bot = Load.photonClients.Find(pc => pc.IsConnectedAndReady && pc.CloudRegion.Contains("jp"));
                        else if (worldPath.Contains("~region(eu)")) Bot = Load.photonClients.Find(pc => pc.IsConnectedAndReady && pc.CloudRegion.Contains("eu"));
                        if (worldPath.Contains("wrld_")) Bot.JoinRoom(worldPath);
                        break;
                }
            }
        }

        public bool LeaveRoom()
        {
            return OpLeaveRoom(false);
        }

        public bool JoinRoom(string roomdata, int cap = 0)
        {
            string Token = instance.GetToken();
            if (InRoom)
            {
                LeaveRoom();
                Thread.Sleep(3000);
                JoinRoom(roomdata, cap);
            }
            string[] tempSplit = roomdata.Split(':');
            if (cap == 0) cap = Misc.FetchWorld(tempSplit[0], Token).capacity;
            string roomPassword = Misc.FetchRoomToken(roomdata, Token, $"usr_{UserId}");
            if (roomPassword == null || cap == 0)
            {
                Logger.LogError("No Roomtoken / Capacity ");
                return false;
            }

            EnterRoomParams enterRoomParams = new EnterRoomParams
            {
                CreateIfNotExists = true,
                RejoinOnly = false,
                RoomName = roomdata,
            };
            RoomOptions roomOptions = new RoomOptions
            {
                IsOpen = true,
                IsVisible = false,
                MaxPlayers = Convert.ToByte(cap * 2)
            };
            Hashtable hashtable = new Hashtable
            {
                { (byte)3, 1 },
                { (byte)2, roomPassword }
            };
            roomOptions.CustomRoomProperties = hashtable;
            enterRoomParams.RoomOptions = roomOptions;
            roomOptions.EmptyRoomTtl = 0;
            roomOptions.PublishUserId = false;
            return OpJoinRoom(enterRoomParams);
        }

        public void OnConnected()
        {
            Logger.Log("OnConnected Called");
        }

        public void OnConnectedToMaster()
        {
            LocalPlayer.SetCustomProperties(new Hashtable()
            {
                { "inVRMode", true },
                { "showSocialRank", true },
                { "steamUserID", "0" },
                { "modTag", null},
                { "isInvisible", false},
                { "avatarEyeHeight", new Random().Next(1, 6666)},
            });
            Logger.Log($"{instance.UserId} Connected to Masterserver [{LocalPlayer.CustomProperties.Count}]");
        }

        public void OnCreatedRoom()
        {
            Logger.Log($"{instance.UserId} Room Created");
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            Logger.LogError($"{instance.UserId} Failed to create Room");
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
            Logger.LogError($"Failed to authenticate for {instance.AuthValues.AuthGetParameters} {debugMessage}");
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            Logger.Log("Authenticated as " + instance.AuthValues.AuthGetParameters);
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            Logger.LogWarning($"Photon {instance.UserId} Disconnected -> {cause}");
            ReconnectToMaster();
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            Logger.Log("OnFriendListUpdate Called");
        }

        public void OnJoinedLobby()
        {
            Logger.Log("OnJoinedLobby Called");
        }

        public static bool ShouldInstantiate = false;
        public static bool InstantiateSilent = false;
        public static bool ShouldLog = false;

        public void OnJoinedRoom()
        {
            Logger.Log($"RoomName: {CurrentRoom.Name}");
            Logger.LogSuccess($"{instance.UserId} connected to Room [{CurrentRoom.PlayerCount}] [{instance.CloudRegion}]");
            //string InstanceToken = HttpUtils.ValidateInstance(CurrentRoom.Name, instance.GetToken());
            //if (InstanceToken != null) Logger.LogDebug($"Received Token: {InstanceToken}");
            //else Logger.LogError($"No Validation Token for Instance ");
            string[][] bytes = Serialization.FromByteArray<string[][]>(Convert.FromBase64String("AAEAAAD/////AQAAAAAAAAAHAQAAAAEBAAAAAgAAAAYJAgAAAAkDAAAAEQIAAAAAAAAAEQMAAAAAAAAACw=="));
            OpRaiseEvent(33, new Dictionary<byte, object>
                {
                    { 0, (byte)20 },
                    { 3, bytes},
                }, new RaiseEventOptions()
                {
                    CachingOption = EventCaching.DoNotCache,
                    Receivers = ReceiverGroup.Others,
                }, new SendOptions()
                {
                    DeliveryMode = DeliveryMode.Reliable,
                    Reliability = true,
                    Channel = 0,
                });
            if (ShouldInstantiate) Instantiate(InstantiateSilent);
            if (Search.ShouldCheckRoom)
            {
                while (CurrentRoom.Players == null) Thread.Sleep(200);
                string Room = instance.CurrentRoom.Name;
                foreach (var Player in CurrentRoom.Players)
                {
                    if (Player.Value != null) Search.CheckInstanceUsers(Room, Player.Value);
                }
                Thread.Sleep(2000);
                LeaveRoom();
            }
            else if (ShouldLog)
            {
                ShouldLog = false;
                while (CurrentRoom.Players == null) Thread.Sleep(200);
                Logger.Log(instance.CurrentRoom.Name);
                Logger.Log("");
                foreach (var Player in CurrentRoom.Players)
                {
                    if (Player.Value != null) Logger.Log($"{Player.Value.GetDisplayName()} || {Player.Value.GetAPIUserID()} || {Player.Value.ActorNumber}");
                }
                Logger.Log("");
                Thread.Sleep(2000);
                LeaveRoom();
            }
        }

        public void Instantiate(bool Master)
        {
            if (InstantiateSelf(Master))
            {
                //Exploits.SendEvent8(instance);
                Logger.LogSuccess($"{instance.UserId} instantiated");
            }
        }

        public bool InstantiateSelf(bool MasterOnly)
        {
            //Creating instantiate parameters
            InstantiateParameters parameters = new InstantiateParameters("VRCPlayer", new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 1f), 0, null, 0,
                new int[3]
                {
                    int.Parse(LocalPlayer.ActorNumber + "00001"),
                    int.Parse(LocalPlayer.ActorNumber + "00002"),
                    int.Parse(LocalPlayer.ActorNumber + "00003")
                },
                LocalPlayer,
                LoadBalancingPeer.ServerTimeInMilliSeconds);

            //Instantiation ID
            int intID = parameters.viewIDs[0];

            SendInstantiateEvHashtable.Clear();
            SendInstantiateEvHashtable[(byte)0] = parameters.prefabName;
            SendInstantiateEvHashtable[(byte)1] = parameters.position;
            SendInstantiateEvHashtable[(byte)2] = parameters.rotation;
            if (parameters.viewIDs.Length > 1) SendInstantiateEvHashtable[(byte)4] = parameters.viewIDs;
            SendInstantiateEvHashtable[(byte)6] = parameters.timestamp;
            SendInstantiateEvHashtable[(byte)7] = intID;

            //Adding our instantiation to the Roomcache
            SendInstantiateRaiseEventOptions = RaiseEventOptions.Default;
            SendInstantiateRaiseEventOptions.CachingOption = EventCaching.AddToRoomCache;
            if (MasterOnly) SendInstantiateRaiseEventOptions.Receivers = ReceiverGroup.MasterClient;
            else SendInstantiateRaiseEventOptions.Receivers = ReceiverGroup.Others;
            //Finally calling OpRaiseEvent to send it over the network
            return OpRaiseEvent(202, SendInstantiateEvHashtable, SendInstantiateRaiseEventOptions, SendOptions.SendReliable);
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            Logger.LogError($"{instance.UserId} Failed to join random room Code:{returnCode} Message:{message}");
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            Logger.LogError($"{instance.UserId} Failed to join room Code:{returnCode} Message:{message}");
        }

        public void OnLeftLobby()
        {
            Logger.LogWarning($"{instance.UserId} left the Lobby");
        }

        public void OnLeftRoom()
        {
            Logger.LogWarning($"{instance.UserId} left the Room");
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            Logger.Log($"New MasterClient: {newMasterClient.GetDisplayName()}");
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }
    }

    public struct InstantiateParameters
    {
        public int[] viewIDs;
        public byte objLevelPrefix;
        public object[] data;
        public byte @group;
        public Quaternion rotation;
        public Vector3 position;
        public string prefabName;
        public Player creator;
        public int timestamp;

        public InstantiateParameters(string prefabName, Vector3 position, Quaternion rotation, byte @group, object[] data, byte objLevelPrefix, int[] viewIDs, Player creator, int timestamp)
        {
            this.prefabName = prefabName;
            this.position = position;
            this.rotation = rotation;
            this.@group = @group;
            this.data = data;
            this.objLevelPrefix = objLevelPrefix;
            this.viewIDs = viewIDs;
            this.creator = creator;
            this.timestamp = timestamp;
        }
    }
}