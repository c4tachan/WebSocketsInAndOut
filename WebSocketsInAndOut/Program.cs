using System;


namespace WebSocketsInAndOut
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            SocketController socketController = new SocketController("127.0.0.1", 80);

            socketController.SocketProcessLoop();


        }
    }
}
