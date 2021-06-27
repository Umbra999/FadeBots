using FadeBots.Client;
using FadeBots.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FadeBot.Functions
{
    public class Load
    {
        public static List<PhotonClient> photonClients = new List<PhotonClient>();
        public static List<string> Authcookies = new List<string>();
        public static bool WebsocketMode = false;

        public static void Startup()
        {
            HttpUtils.HWID = HWIDSpoof.GenerateHWID();
            Misc.SendVisitLoop();

            if (WebsocketMode)
            {
                Websocket.Initialize();
                if (File.Exists("FadeAway\\Photon\\Userpass.txt")) Misc.ReqAuthMethod();
                else Logger.LogWarning("No Userpass found -> Skip");
            }
            else
            {
                Logger.LogImportant("1 - Userpass Auth");
                Logger.LogImportant("2 - Token Auth");
                Logger.LogImportant("3 - Convert Userpass to Token");
                string Input = Console.ReadLine();
                if (Input == "1")
                {
                    if (File.Exists("Userpass.txt")) Misc.ReqAuthMethod();
                    else Logger.LogWarning("No Userpass found -> Skip");
                }
                else if (Input == "2")
                {
                    if (File.Exists("Tokens.txt")) Misc.AuthCacheMethod();
                    else Logger.LogWarning("No Tokens found -> Skip");
                }
                else if (Input == "3")
                {
                    if (File.Exists("Userpass.txt")) Misc.ConvertToToken();
                    else Logger.LogWarning("No Userpass found -> Skip");
                }
                Thread.Sleep(2000);
            }
        }
    }
}