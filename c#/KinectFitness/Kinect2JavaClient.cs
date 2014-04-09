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
        String flag;

        public Kinect2JavaClient(String message)
        {
            this.flag = message;
        }
        // code from http://stackoverflow.com/questions/8413096/how-do-i-use-socket-programming-to-send-messages

        public void sendFlag()
        {
            TcpClient socketForServer;
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
            var streamWriter = new System.IO.StreamWriter(networkStream);

            try
            {
                // read the data from the host and display it
                {
                    streamWriter.WriteLine(flag+"\n");
                    streamWriter.Flush();
                }
            }
            catch
            {
                Console.WriteLine("Exception reading from Server");
            }
            // tidy up
            streamWriter.Close();
            networkStream.Close();
        }
    }
}
