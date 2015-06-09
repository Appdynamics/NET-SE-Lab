using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace TcpCommLib
{
    public class Server
    {
        public delegate Message OnMessageHandler(Message message);

        TcpListener listener;
        int port;
        bool isActive = false;
        OnMessageHandler onMessageHandler;

        public Server(int port, OnMessageHandler handler)
        {
            this.port = port;
            this.onMessageHandler += handler;
        }

        public void Start()
        {
            listener = TcpListener.Create(port);
            listener.Start();
            isActive = true;

            Task.Factory.StartNew(() => { ProcessRequests(); });
        }

        private void ProcessRequests()
        {
            while (isActive)
            {
                if (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient();

                    Task.Factory.StartNew(() => { HandleRequest(client); });
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        private void HandleRequest(TcpClient client)
        {
            try
            {
                using (client)
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        Message message = Message.ReadFromStream(stream);

                        Message reply = onMessageHandler(message);

                        reply.WriteToStream(stream);

                        stream.Flush();

                        //while (client.Available == 0) System.Threading.Thread.Sleep(1);
                        //while (stream.ReadByte() != -1) System.Threading.Thread.Sleep(1);

                        //client.Client.Shutdown(SocketShutdown.Both);
                        client.Client.Disconnect(false);
                    }
                }
            }
            catch(Exception ex)
            {
                string s = ex.ToString();
            }
        }
        

        public void Stop()
        {
            if(listener != null)
            {
                isActive = false;
                listener.Stop();
                listener = null;
            }
        }
    }
}
