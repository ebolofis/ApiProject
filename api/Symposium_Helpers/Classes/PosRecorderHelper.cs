using SimpleTCP;
using Symposium.Models.Models.DahuaRecorder;
using System;
using System.Text;

namespace Symposium.Helpers.Classes
{
    public class PosRecorderHelper
    {
        public static SimpleTcpClient client = new SimpleTcpClient();
       

        public void InitializeConnection(string ipAddress, string port)
        {
            client.StringEncoder = Encoding.Default;
            //Make Tcp/Ip Connection with XVR Recoder
            client.Connect(ipAddress, Convert.ToInt32(port));

            return;
        }

        /// <summary>
        /// Send Message to Server 
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public void SendMessageToServer(PosRecorderModel Model)
        {
            client.WriteLineAndGetReply(Model.Message, TimeSpan.FromSeconds(3));
            return;
        }
    }
}
