using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TcpApp_Classes;

using var tcpClient = new TcpClient();
await tcpClient.ConnectAsync(IPAddress.Loopback, 5005);

using var stream = tcpClient.GetStream();

while (true)
{
    using var reader = new StreamReader(stream, new UTF8Encoding(false));
    using var writer = new StreamWriter(stream, new UTF8Encoding(false));
    try
    {
        //Request (send)
        Console.Write("Input your message: ");
        var chatObject = CreateChatObject(Console.ReadLine());

        if (chatObject.Message == null)
            chatObject.Message = "<<EMPTY>>";


        var json = JsonSerializer.Serialize(chatObject);

        await writer.WriteAsync(json);
        await writer.FlushAsync();

        //Response (receive)
        Console.WriteLine("Message sent. Waiting for response.. ");

        var jsonResponse = string.Empty;
        
        while (!reader.EndOfStream)
        {
            jsonResponse = await reader.ReadToEndAsync();
        }
        

        var responseObject = JsonSerializer.Deserialize<ChatObject>(jsonResponse);

        Console.WriteLine(
            $"[{chatObject.TimeStamp}] " +
            $"Received -> '{chatObject.Message}' \n" +
            $"from {chatObject.SourceName}");

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
