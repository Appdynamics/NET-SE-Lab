using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib
{
    public class Client
    {
        int port;
        string hostname;

        public Client(int port, string hostname)
        {
            this.hostname = hostname;
            this.port = port;
        }

        public void Put(string key, string value)
        {
            TcpCommLib.Client client = new TcpCommLib.Client(hostname, port);
            TcpCommLib.Message message = new TcpCommLib.Message();

            message.Values["method"] = "put";
            message.Values["key"] = key;
            message.Values["value"] = value;

            TcpCommLib.Message reply = client.SendMessage(message);

            if(!string.IsNullOrEmpty(reply.Values["exception"]))
            {
                throw new Exception(reply.Values["exception"]);
            }
        }

        public string Get(string key)
        {
            TcpCommLib.Client client = new TcpCommLib.Client(hostname, port);
            TcpCommLib.Message message = new TcpCommLib.Message();

            message.Values["method"] = "get";
            message.Values["key"] = key;

            TcpCommLib.Message reply = client.SendMessage(message);

            if (!string.IsNullOrEmpty(reply.Values["exception"]))
            {
                throw new Exception(reply.Values["exception"]);
            }

            return reply.Values["value"];
        }
    }
}
