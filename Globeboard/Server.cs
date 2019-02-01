using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.IO;

namespace Globeboard
{
    class Server
    {
		private const int PORT = 37001;
        private IPAddress iPAddress = null;
        TcpListener listener = null;
        Socket socket = null;
        //Stream networkStream = null;

        public Server(IPAddress address) {
            iPAddress = address;
        }

        public void Start()
        {
            listener = new TcpListener(iPAddress, PORT);
            listener.Start();
            while (true)
            {
				socket = listener.AcceptSocket();
                //networkStream = new NetworkStream(socket);
                byte[] len = new byte[4];
                byte[] data = new byte[1024];
                int length = 0;
                string dataStr = "";
                do
                {
                    socket.Receive(len, 4, SocketFlags.None);
                    length = BitConverter.ToInt32(len, 0);
                    socket.Receive(data, length, SocketFlags.None);
                    dataStr += System.Text.Encoding.ASCII.GetString(data);
                } while (dataStr.IndexOf('\0') < 0);
                Console.WriteLine(dataStr);
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("Server: Chimichanga");
                int msgLen = msg.Length;
                byte[] msgLenBytes = new byte[4];
                int offset = 0;
                while (msgLen > 1024)
                {
                    msgLenBytes = BitConverter.GetBytes(1024);
                    socket.Send(msgLenBytes, 4, SocketFlags.None);
                    socket.Send(msg, offset, 1024, SocketFlags.None);
                    msgLen -= 1024;
                }
                msgLenBytes = BitConverter.GetBytes(msgLen);
                socket.Send(msgLenBytes, 4, SocketFlags.None);
                socket.Send(msg, offset, msgLen, SocketFlags.None);
            }
        }
    }
}
