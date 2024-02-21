using System.Net;
using System.Net.Sockets;

namespace Seminar1;

internal class Program
{
  static async Task Main(string[] args)
  {
    var cs = new ChatServer();

    await cs.Run();
  }
}

public class ChatServer
{
  TcpListener listener = new TcpListener(IPAddress.Any, 55555);
  static List<TcpClient> clients = new List<TcpClient>();

  public async Task Run()
  {
    try
    {
      listener.Start();
      await Console.Out.WriteLineAsync("Сервер запущен");

      while (true)
      {
        var tcpClient = await listener.AcceptTcpClientAsync();
        await Console.Out.WriteLineAsync($"{tcpClient.GetHashCode()} подключен!");

        _ = Task.Run(() => ProcessClient(tcpClient));

        _ = Task.Run(() => SendMessageFromClient(tcpClient));
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }

  public static async Task ProcessClient(TcpClient tcpClient)
  {

    if (!clients.Contains(tcpClient))
    {
      clients.Add(tcpClient);

      foreach (var client in clients)
      {
        if (client.Connected && client.GetHashCode() != tcpClient.GetHashCode())
        {
          StreamWriter writer = new StreamWriter(client.GetStream());

          await writer.WriteLineAsync(tcpClient.GetHashCode() + " присоединился");
          writer.Flush();
        }
      }
    }
  }

  public static async Task SendMessageFromClient(TcpClient tcpClient)
  {

    try
    {
      var reader = new StreamReader(tcpClient.GetStream());
      string? message;
    
      while (tcpClient.Connected)
      {
        if (!string.IsNullOrEmpty(message = reader.ReadLine()))
        {

          foreach (var client in clients)
          {
            if (client.Connected && client.GetHashCode() != tcpClient.GetHashCode())
            {
              StreamWriter writer = new StreamWriter(client.GetStream());

              await writer.WriteLineAsync(tcpClient.GetHashCode() + ": " + message);
              writer.Flush();
            }
          }
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }
}
