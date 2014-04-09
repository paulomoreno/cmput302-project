using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Threading;

namespace KinectFitness
{
    class PatientTcpServer
    {
        static TcpListener tcpListener;
        static String heartRate;

        public static void getHeartRate()
        {
            IPAddress ipAddress = Dns.Resolve("127.0.0.1").AddressList[0];
            MessageBox.Show(ipAddress.ToString());
            tcpListener = new TcpListener(ipAddress, 7003);
            tcpListener.Start();
      
            //create a new thread and insert this code there! otherwise it will freeze up the entire UI
            Thread thread = new Thread(PatientTcpServer.listenToYourHeart);
            thread.Start();
            //client.Close();
            //tcpListener.Stop();
        }

        private static void listenToYourHeart()
        {
            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                NetworkStream ns = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];
                int bytesRead = ns.Read(buffer, 0, client.ReceiveBufferSize);
                heartRate = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                MessageBox.Show(heartRate);
            }
        }
    }
}
