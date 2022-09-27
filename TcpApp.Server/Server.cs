using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Xml;
using TcpApp_Classes;

var endpoint = new IPEndPoint(IPAddress.Loopback, 5005);
var listener = new TcpListener(endpoint); //Skapa en endpoint att lyssna på och en TcpListener som lyssnar på den
Console.WriteLine("Welcome to the TCP Echo-Chamber!");
listener.Start();
Console.WriteLine("Listening");
//App väntar på en connection
using var tcpClient = await listener.AcceptTcpClientAsync();
using var stream = tcpClient.GetStream();
//När en klient kopplar upp accepteras kopplingen
//Ur TcpClient skapas StreamReader och StreamWriter för att läsa och skriva till strömmen
Console.WriteLine("Accepted connection");

while (tcpClient.Connected)
{
    using var reader = new StreamReader(stream, new UTF8Encoding(false));
    using var writer = new StreamWriter(stream, new UTF8Encoding(false));
    
    try
    {
        //Request
        var json = string.Empty;
        Console.WriteLine("Waiting for a message... ");
        while (!reader.EndOfStream)
        {
            json = await reader.ReadToEndAsync();
        }
        

        var chatObject = JsonSerializer.Deserialize<ChatObject>(json);
        Console.WriteLine(
        $"[{chatObject.TimeStamp}] " +
        $"Received -> '{chatObject.Message}' \n" +
        $"from {chatObject.SourceName}");

        //Response

        Console.Write("Input response: ");
        var responseObject = CreateChatObject(Console.ReadLine());

        if (responseObject.Message == null)
            responseObject.Message = "<<EMPTY>>";

        var data = JsonSerializer.Serialize(responseObject);

        await writer.WriteAsync(data);
        await writer.FlushAsync();
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