using FadeBot.Functions;
using System;
using System.Threading;

namespace FadeBot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Load.Startup();
            if (!Load.WebsocketMode) new Thread(PhotonClient.UpdateClient).Start();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        }

        public static void OnProcessExit(object sender, EventArgs e)
        {
            foreach (PhotonClient client in Load.photonClients)
            {
                if (client.InRoom) client.LeaveRoom();
            }
            Thread.Sleep(2000);
        }
    }
}