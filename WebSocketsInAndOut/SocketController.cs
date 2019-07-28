using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WebSocketsInAndOut
{
    class SocketController
    {
        private TcpListener listner;

        public SocketController(string ipAddress, int port)
        {
            listner = new TcpListener(IPAddress.Parse(ipAddress), port);

            listner.Start();
            Console.WriteLine("A TCP Listner has been started on {0}:{1}", ipAddress, port);
        }

        public void SocketProcessLoop()
        { 
            TcpClient client = listner.AcceptTcpClient();

            Console.WriteLine("A client connected.");

            NetworkStream stream = client.GetStream();

            while(true)
            {
                while (!stream.DataAvailable) ; // Loop until there is data available!

                Byte[] bytes = new Byte[client.Available];

                stream.Read(bytes, 0, bytes.Length);

                String data = Encoding.UTF8.GetString(bytes);

                if (Regex.IsMatch(data, "^GET"))
                {
                    const string eol = "\r\n"; // HTTP/1.1 defines the sequence CR LF as the end-of-line marker

                    Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
                        + "Connection: Upgrade" + eol
                        + "Upgrade: websocket" + eol
                        + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                            System.Security.Cryptography.SHA1.Create().ComputeHash(
                                Encoding.UTF8.GetBytes(
                                    new System.Text.RegularExpressions.Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                                )
                            )
                        ) + eol
                        + eol);

                    stream.Write(response, 0, response.Length);

                    Console.WriteLine("Handshake complete!");
                }
                else
                {
                    int payloadLength = bytes[1] - 128;

                    Byte[] mask = new Byte[4] { bytes[2], bytes[3], bytes[4], bytes[5] };
                    Byte[] decoded = new Byte[payloadLength];
                    for (int i = 0; i < payloadLength; i++)
                    {
                        decoded[i] = (Byte)(bytes[i + 6] ^ mask[i % 4]);
                    }
                    String stringData = Encoding.UTF8.GetString(decoded);
                    Console.WriteLine(stringData);
                }

                
            }
        }
    }
}
