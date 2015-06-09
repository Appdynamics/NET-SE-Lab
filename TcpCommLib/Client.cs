using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace TcpCommLib
{
    public class Client
    {
        
        string hostname;
        int port;

        public Client(string hostname, int port)
        {
            this.hostname = hostname;
            this.port = port;
        }

        public Message SendMessage(Message message)
        {
            Message reply;

            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(hostname, port);

                    using (NetworkStream stream = client.GetStream())
                    {
                        message.WriteToStream(stream);

                        stream.Flush();

                        reply = Message.ReadFromStream(stream);

                        // client.Client.Shutdown(SocketShutdown.Both);
                        client.Client.Disconnect(false);
                    }
                }
            }
            catch(Exception ex)
            {
                reply = new Message();
            }

            return reply;
        }
    }
}
