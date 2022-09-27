using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Text;

using var tcpClient = new TcpClient();
await tcpClient.ConnectAsync("localhost", 5005);

using var stream = tcpClient.GetStream();

using var reader = new StreamReader(stream, new UTF8Encoding(false));
using var writer = new StreamWriter(stream, new UTF8Encoding(false));


while (tcpClient.Connected)
{
    Console.Write("Input your message: ");
    var input = Console.ReadLine();

    await writer.WriteAsync(input);
    await writer.FlushAsync();

    //Thread.Sleep(1000);

    while (!reader.EndOfStream)
    {
        var response = await reader.ReadToEndAsync();

        Console.WriteLine(response);
        break;
    }

}

// writer.FlushAsync

