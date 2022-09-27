using System.Net;
using System.Net.Sockets;
using System.Text;

var endpoint = new IPEndPoint(IPAddress.Loopback, 5005);

var listener = new TcpListener(endpoint);

listener.Start();

Console.WriteLine("Listening");

using var tcpClient = listener.AcceptTcpClient();

Console.WriteLine("Accepted connection");

while (true)
{

    var stream = tcpClient.GetStream();
    var client = tcpClient.Client;

    //Request part
    var buffer = new byte[2048];
    var lengthReceived = await client.ReceiveAsync(buffer, SocketFlags.None);

    var data = buffer.Take(lengthReceived).ToArray();

    Console.WriteLine($"Received: {Encoding.UTF8.GetString(data)}");

    //Thread.Sleep(1000);
    //Response part

    var response = $"Server received: {Encoding.UTF8.GetString(data)}";


    Console.WriteLine("Sending response..");
    await writer.WriteAsync(response);
    await writer.FlushAsync();
    Console.WriteLine($"Send complete.");
}
Console.WriteLine("Connection terminated.");