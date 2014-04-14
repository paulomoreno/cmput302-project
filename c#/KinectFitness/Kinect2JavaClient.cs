using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace KinectFitness
{
    class Kinect2JavaClient
    {
        public static TcpClient socketForServer;

        // code from http://stackoverflow.com/questions/8413096/how-do-i-use-socket-programming-to-send-messages

        public void receiveData()
        {

            try
            {
                socketForServer = new TcpClient("localHost", 5001);
            }
            catch
            {
                Console.WriteLine(
                "Failed to connect to server at {0}:999", "localhost");
                return;
            }

            NetworkStream networkStream = socketForServer.GetStream();
            var streamReader = new System.IO.StreamReader(networkStream);
            //var streamWriter = new System.IO.StreamWriter(networkStream);

            try
            {
                while (streamReader.ReadLine() != null)
                {
                    String data = streamReader.ReadLine();
                    Console.WriteLine(data);
                }
            }
            catch
            {
                Console.WriteLine("Exception reading from Server");
            }
            // tidy up
            streamReader.Close();
            networkStream.Close();
        }
    }
}