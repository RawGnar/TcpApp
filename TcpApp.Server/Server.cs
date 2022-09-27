using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Xml;
using TcpApp_Classes;

var endpoint = new IPEndPoint(IPAddress.Any, 5005);
var listener = new TcpListener(endpoint); //Skapa en endpoint att lyssna på och en TcpListener som lyssnar på den

Console.WriteLine("Welcome to the TCP Echo-Chamber!");

listener.Start();

//App väntar på en connection
Console.WriteLine("Listening");
using var tcpClient = await listener.AcceptTcpClientAsync();
using var handler = tcpClient.Client;
//När en klient kopplar upp accepteras kopplingen
//Ur TcpClient skapas StreamReader och StreamWriter för att läsa och skriva till strömmen
Console.WriteLine("Accepted connection");

while (tcpClient.Connected)
{
    
    try
    {
        //Request
        byte[] buffer = new byte[1024];
        var length = await handler.ReceiveAsync(buffer, SocketFlags.None);
        
        var data = buffer.Take(length).ToArray();

        var chatObject = JsonSerializer.Deserialize<ChatObject>(data);

        Console.WriteLine(
        $"[{chatObject.TimeStamp}] " +
        $"Received -> '{chatObject.Message}' \n" +
        $"from {chatObject.SourceName}");

        //Response

        Console.Write("Input response: ");
        var responseObject = CreateChatObject(Console.ReadLine());

        if (responseObject.Message == null)
            responseObject.Message = "<<EMPTY>>";

        var json = JsonSerializer.Serialize(responseObject);

        await handler.SendAsync(Encoding.UTF8.GetBytes(json), SocketFlags.None);
        Console.WriteLine($"Send complete.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.Read();
        break;
    }

}
Console.WriteLine("Connection terminated.");


ChatObject CreateChatObject(string message)
{
    return new ChatObject(message, "Server");
}