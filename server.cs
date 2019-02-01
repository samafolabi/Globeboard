using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.IO;

namespace servertest {

	class server {
		static void Main(string[] args) {
			try {
				IPAddress ip = null;
				foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList) {
					if (address.AddressFamily == AddressFamily.InterNetwork) {
						ip = address;
						Console.WriteLine(address);
						break;
					} 
				}
				TcpListener listener = new TcpListener(ip, 447);
				listener.Start();
				Socket socket = listener.AcceptSocket();
				Stream networkStream = new NetworkStream(socket);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
			Console.Read();
		}
	}

}