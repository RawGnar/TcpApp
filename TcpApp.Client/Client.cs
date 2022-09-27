using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TcpApp_Classes;

using var tcpClient = new TcpClient();
await tcpClient.ConnectAsync(IPAddress.Loopback, 5005);
using var connection = tcpClient.Client;

while (tcpClient.Connected)
{
    try
    {
        //Request (send)
        Console.Write("Input your message: ");
        var chatObject = CreateChatObject(Console.ReadLine());

        if (chatObject.Message == null)
            chatObject.Message = "<<EMPTY>>";

        var json = JsonSerializer.Serialize(chatObject);

        await connection.SendAsync(Encoding.UTF8.GetBytes(json), SocketFlags.None);

        //Response (receive)
        Console.WriteLine("Message sent. Waiting for response.. ");

        var buffer = new byte[1024];
        var length = await connection.ReceiveAsync(buffer, SocketFlags.None);

        var data = buffer.Take(length).ToArray();
        

        var responseObject = JsonSerializer.Deserialize<ChatObject>(Encoding.UTF8.GetString(data));

        Console.WriteLine(
            $"[{responseObject.TimeStamp}] " +
            $"Received -> '{responseObject.Message}' \n" +
            $"from {responseObject.SourceName}");

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.Read();
    }
}

ChatObject CreateChatObject(string message)
{
    return new ChatObject(message, "Client");
}
