using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib
{
    public class Server
    {
        TcpCommLib.Server server;
        int port;

        public Server(int port)
        {
            this.port = port;
        }

        public void Start()
        {
            server = new TcpCommLib.Server(port, this.ProcessRequest);
            server.Start();
        }

        public void Stop()
        {
            if (server != null)
            {
                server.Stop();
                server = null;
            }
        }

        private TcpCommLib.Message ProcessRequest(TcpCommLib.Message message)
        {
            switch(message.Values["method"])
            {
                case "get":
                    return Get(message);

                case "put":
                    return Put(message);

                default:
                    TcpCommLib.Message reply = new TcpCommLib.Message();
                    reply.Values["exception"] = "Method is not implemented.";
                    return reply;
            }
        }

        IDictionary<string, string> cache = new ConcurrentDictionary<string, string>();

        private TcpCommLib.Message Get(TcpCommLib.Message message)
        {
            TcpCommLib.Message reply = new TcpCommLib.Message();

            try
            {
                string key = message.Values["key"];

                if (cache.ContainsKey(key))
                    reply.Values["value"] = cache[key];
                else
                    reply.Values["value"] = "";
            }
            catch(Exception ex)
            {
                reply.Values["exception"] = ex.ToString();
            }

            return reply;
        }

        private TcpCommLib.Message Put(TcpCommLib.Message message)
        {
            TcpCommLib.Message reply = new TcpCommLib.Message();
            
            try
            {
                string key = message.Values["key"];
                cache[key] = message.Values["value"];

                reply.Values["result"] = "Cache updated";
            }
            catch (Exception ex)
            {
                reply.Values["exception"] = ex.ToString();
            }

            return reply;
        }
    }
}
