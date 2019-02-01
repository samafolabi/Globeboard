using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Globeboard
{
    public class Client
    {
		private const int PORT = 37001;
        private IPAddress iPAddress = null;
        private string ipRange = "";
        private List<string> ipAddresses = new List<string>(20);
        private Socket s = null;

        public Client(IPAddress address)
        {
            iPAddress = address;
            ipRange = Convert.ToString(address).Substring(0, Convert.ToString(address).LastIndexOf('.') + 1);
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void DiscoverRange() {
            ProcessStartInfo cmd = new ProcessStartInfo("cmd", "/c nmap -sP " + (ipRange + "0/24"));
            cmd.RedirectStandardOutput = true;
            cmd.UseShellExecute = false;
            cmd.CreateNoWindow = true;
            Process proc = new Process();
            proc.StartInfo = cmd;
            proc.Start();
            string line;
            while ((line = proc.StandardOutput.ReadLine()) != null)
            {
                if (line.Contains(ipRange))
                {
                    string test = line.Substring(line.IndexOf(ipRange));
                    if (!System.Text.RegularExpressions.Regex.IsMatch("" + test[test.Length - 1], "[0-9]"))
                    {
                        test = test.Remove(test.Length - 1);
                    }
                    ipAddresses.Add(test);
                }
            }
            Console.WriteLine("Client Discover Range: " + ipAddresses);
        }

        public void Discover()
        {
            foreach (string ip in ipAddresses)
            {
                Console.WriteLine("Client Discover: " + ip);
                IPAddress address = IPAddress.Parse(ip);
                IPEndPoint endPoint = new IPEndPoint(address, PORT);
                s.Connect(endPoint);
                s.Send(System.Text.Encoding.ASCII.GetBytes("Client: Hello World"));
                byte[] data = new byte[1024];
                string dataStr = "";
                do
                {
                    s.Receive(data);
                    dataStr += System.Text.Encoding.ASCII.GetString(data);
                } while (dataStr.IndexOf("<EOF>") < 0);

                Console.WriteLine(dataStr);
            }
        }
            
        
    }
}

