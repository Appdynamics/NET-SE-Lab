using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.IO;

namespace TcpCommLib
{
    public class Message
    {
        private NameValueCollection values;

        private Message(NameValueCollection values)
        {
            this.values = values;
        }

        public Message()
        {
            values = new NameValueCollection();
        }

        public NameValueCollection Values
        {
            get
            {
                return values;
            }
        }

        public void WriteToStream(Stream stream)
        {
            List<string> items = new List<string>();
            foreach (string name in values)
                items.Add(string.Concat(name, "=", System.Web.HttpUtility.UrlEncode(values[name])));

            string data = string.Join("&", items);

            byte[] bytes = Encoding.UTF8.GetBytes(data);
            
            byte[] header = BitConverter.GetBytes(bytes.Length);

            stream.Write(header, 0, header.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static Message ReadFromStream(Stream stream)
        {
            byte[] header = new byte[4];
            stream.Read(header, 0, 4);

            int length = BitConverter.ToInt32(header, 0);

            if (length == 0)
                return new Message(new NameValueCollection());

            byte[] bytes = new byte[length];
            int l = stream.Read(bytes, 0, bytes.Length);

            string data = Encoding.UTF8.GetString(bytes, 0, l);

            NameValueCollection values = System.Web.HttpUtility.ParseQueryString(data);
            return new Message(values);
        }
    }
}
