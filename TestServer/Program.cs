using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CacheLib.Server server = new CacheLib.Server(5000);
            server.Start();

            Console.ReadLine();

            server.Stop();
        }
    }
}
