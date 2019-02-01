using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.IO;
using System.Diagnostics;

namespace servertest {

	class client {
		static void Main(string[] args) {
			try {
				IPAddress ipA = null;
				foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList) {
					if (address.AddressFamily == AddressFamily.InterNetwork) {
						ipA = address;
						Console.WriteLine(address);
						break;
					} 
				}
				string ip = Convert.ToString(ipA);
				string ipRange = ip.Substring(0, ip.LastIndexOf('.')+1);
				Console.WriteLine(ipRange);
				ProcessStartInfo cmd = new ProcessStartInfo("cmd", "/c nmap -sP " + (ipRange + "0/24"));
				cmd.RedirectStandardOutput = true;
				cmd.UseShellExecute = false;
				cmd.CreateNoWindow = true;
				Process proc = new Process();
				proc.StartInfo = cmd;
				proc.Start();
				string result = "";
				string line;
				while ((line = proc.StandardOutput.ReadLine()) != null) {
					if (line.Contains(ipRange)) {
						string test = line.Substring(line.IndexOf(ipRange));
						if (!System.Text.RegularExpressions.Regex.IsMatch(""+test[test.Length-1], "[0-9]")) {
							test = test.Remove(test.Length-1);
						}
						result += test + '\n';
					}
				}
				Console.WriteLine(result);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
			Console.Read();
		}
	}

}