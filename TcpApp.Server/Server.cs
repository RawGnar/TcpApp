using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Xml;
using TcpApp_Classes;

#region Setup
var endpoint = new IPEndPoint(IPAddress.Loopback, 5005); //Skapa en endpoint att lyssna på
var listener = new TcpListener(endpoint); //och en TcpListener som lyssnar på den
listener.Start();

//App väntar på en connection
Console.WriteLine("Listening");
using var tcpClient = await listener.AcceptTcpClientAsync();
using var handler = tcpClient.Client;
#endregion

#region Inform
//När en klient kopplar upp instansieras en TcpClient och via den en socket för att kommunicera över kopplingen.
Console.WriteLine("Accepted connection");
Console.WriteLine("-----------------------------");
Console.WriteLine("Welcome to TCP-PingPong chat!");
Console.WriteLine("Waiting for a message..");
Console.WriteLine();
#endregion

while (tcpClient.Connected) //Loopar tills kopplingen bryts
{ 
    
    try
    {
        #region Receive
        var buffer = new byte[1024]; //En byte array för att fylla med data från ReceiveAsync metoden behövs.

        //ReceiveAsync fyller buffern med datan och returnerar en int med antalet bytes som tagits emot.
        var length = await handler.ReceiveAsync(buffer, SocketFlags.None);

        var data = buffer.Take(length).ToArray(); //En ny byte array skapas utan tomma platser.

        //Datan som tas emot är en JSON-sträng som kan deserialiseras till ett ChatObject.
        var chatObject = JsonSerializer.Deserialize<ChatObject>(data); 
        //Objektet används för att skriva till konsolen.
        Console.WriteLine(
        $"[{chatObject.TimeStamp}] " +
        $"Received -> '{chatObject.Message}' \n" +
        $"from {chatObject.SourceName}");
        Console.WriteLine();
        #endregion

        #region Send

        Console.Write("Input response('Exit' to quit): ");
        var responseObject = CreateChatObject(Console.ReadLine()); //Användarens input används för att skapa nytt ChatObject.

        if (responseObject.Message.ToLower() == "exit") //Simpel quit funktionalitet
        {
            Console.WriteLine("Closing connection.");
            tcpClient.Close();
            break;
        }

        var json = JsonSerializer.Serialize(responseObject); //Objektet serialiseras till en JSON-sträng

        await handler.SendAsync(Encoding.UTF8.GetBytes(json), SocketFlags.None); //Till sist skickas JSON-strängen iväg som en byte array.
        Console.WriteLine($"Send complete.");
        Console.WriteLine();
        #endregion
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message); //Fångar upp exceptions och skriver felmeddelandet till konsollen för smidigare debugging.
        Console.Read();
    }

}
Console.WriteLine("Connection terminated.");


ChatObject CreateChatObject(string message)
{
    return new ChatObject(message, "Server");
}