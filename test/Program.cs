using System;
using System.IO;
using System.Linq;
using Miniflux;

namespace test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string credential = "credential.txt";
            if (!File.Exists(credential))
            {
                File.Create(credential);
                Console.WriteLine($"Please edit {Path.GetFullPath(credential)} with credential info:");
                return;
            }
            var credentialStr = File.ReadAllLines("credential.txt");
            string URL = credentialStr[0];
            string Username = credentialStr[1];
            string Password = credentialStr[2];
            string Proxy = credentialStr[3];
            MinifluxClient client = new MinifluxClient(URL, Username, Password, Proxy);
            //Do whatever you want.
            var feeds = client.GetFeeds();
            
        }
    }
}