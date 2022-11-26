using System;
using System.Net;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Scruffy
{
    public class ServerConfig
    {
        public string[]? alias { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Scruffy V0");
            
            string config = File.ReadAllText(@"./server.cscru");
            
            Console.WriteLine("Loaded server.cscru");

            HttpListener server = new HttpListener();
            server.Prefixes.Add("http://localhost:8000/");

            server.Start();

            Console.WriteLine("Listening to " + "http://localhost:8000/");

            while (true)
            {
                ScruffyServer.Request(server, config);
            }

        }


    }
}
