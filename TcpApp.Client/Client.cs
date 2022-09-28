using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TcpApp_Classes;

#region Setup
using var tcpClient = new TcpClient();
Console.WriteLine($"Trying to connect to {IPAddress.Loopback}:5005..");
await tcpClient.ConnectAsync(IPAddress.Loopback, 5005);
using var connection = tcpClient.Client; //Skapar upp TcpClient och kopplar upp till den lyssnande servern.
#endregion

#region Inform
Console.WriteLine("Connection successful.");
Console.WriteLine("-----------------------------");
Console.WriteLine("Welcome to TCP-PingPong chat!");
Console.WriteLine("Input 'Exit' to quit.");
Console.WriteLine();
#endregion


while (tcpClient.Connected) //Loopar tills koppling stängs
{
    try
    {
        #region Send
        Console.Write("Input your message: ");

        ChatObject chatObject = CreateChatObject(Console.ReadLine()); //Användarens input används för att skapa nytt ChatObject.

        if (chatObject.Message.ToLower() == "exit") //Simpel quit funktionalitet
        {
            Console.WriteLine("Closing connection.");
            tcpClient.Close();
            break;
        }
        Console.WriteLine();

        var json = JsonSerializer.Serialize(chatObject); //Serialisera Objekt till JSON-sträng

        await connection.SendAsync(Encoding.UTF8.GetBytes(json), SocketFlags.None); //Skicka JSON-sträng som byte array
        #endregion

        #region Receive
        Console.WriteLine("Message sent. Waiting for response.. ");

        var buffer = new byte[1024]; //En byte array för att fylla med data från ReceiveAsync metoden behövs.

        //ReceiveAsync fyller buffern med datan och returnerar en int med antalet bytes som tagits emot
        var length = await connection.ReceiveAsync(buffer, SocketFlags.None); 

        var data = buffer.Take(length).ToArray(); //En ny byte array skapas utan tomma platser.

        //Datan som tas emot deserialiseras till ett ChatObject.
        var responseObject = JsonSerializer.Deserialize<ChatObject>(Encoding.UTF8.GetString(data));
        //Objektet används för att skriva till konsolen.
        Console.WriteLine(
            $"[{responseObject.TimeStamp}] " +
            $"Received -> '{responseObject.Message}' \n" +
            $"from {responseObject.SourceName}");
        Console.WriteLine();
        #endregion
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.Read();
    }
}
Console.WriteLine("Connection terminated.");

ChatObject CreateChatObject(string message)
{
    return new ChatObject(message, "Client");
}
