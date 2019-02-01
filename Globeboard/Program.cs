using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace Globeboard
{
    static class Program
    {
        private static BackgroundWorker worker1 =
            new BackgroundWorker();
        private static BackgroundWorker worker2 =
            new BackgroundWorker();

        private static void worker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker helper = sender as BackgroundWorker;
            Server server = (Server)e.Argument;
            e.Result = serverThread(helper, server);
            Console.WriteLine("Worker1: " + e.Result);
        }

        private static int serverThread(BackgroundWorker bw, Server server)
        {
            server.Start();
            return 0;
        }

        private static void worker2_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker helper = sender as BackgroundWorker;
            Client client = (Client)e.Argument;
            e.Result = clientThread(helper, client);
            Console.WriteLine("Worker2: " + e.Result);
        }

        private static int clientThread(BackgroundWorker bw, Client client)
        {
            client.DiscoverRange();
            client.Discover();
            return 0;
        }

        private static void worker_RunWorkerCompleted(object sender, 
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) Console.WriteLine("Operation was canceled");
            else if (e.Error != null) Console.WriteLine(e.Error.Message);
            else Console.WriteLine(e.Result.ToString());
        }

        static void Main(string[] args)
        {
            try
            {
                worker1.DoWork += new DoWorkEventHandler(worker1_DoWork);
                worker2.DoWork += new DoWorkEventHandler(worker2_DoWork);
                worker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

                IPAddress ip = null;
                foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ip = address;
                        Console.WriteLine(address);
                        break;
                    }
                }

                Server server = new Server(ip);
                Client client = new Client(ip);
                worker1.RunWorkerAsync(server);
                //worker2.RunWorkerAsync(client);
                Console.Read();
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
