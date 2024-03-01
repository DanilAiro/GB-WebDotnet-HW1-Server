using System.Net.Sockets;
using System.Net;

namespace Server
{
  public class ChatServer(IPEndPoint endPoint):IStartable
  {
    TcpListener? _listener = new(endPoint);
    static List<TcpClient> clients = [];

    public async Task Run(CancellationToken ct)
    {
      try
      {
        _listener?.Start();
        Console.WriteLine("Сервер запущен");

        while (true)
        {
          TcpClient? tcpClient = await _listener?.AcceptTcpClientAsync();
          Console.WriteLine($"{tcpClient?.GetHashCode()} подключен!");

          Task.Run(() => ProcessClient(tcpClient), ct);

        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    public static async Task ProcessClient(TcpClient? tcpClient)
    {
      try
      {
        if (!clients.Contains(tcpClient))
        {
          clients.Add(tcpClient);

          foreach (var client in clients)
          {
            if (client.GetHashCode() != tcpClient.GetHashCode())
            {
              StreamWriter writer = new(client.GetStream());

              await writer.WriteLineAsync(tcpClient.GetHashCode() + " присоединился");
              writer.Flush();
            }
          }
        }

        StreamReader reader = new(tcpClient.GetStream());

        while (true)
        {
          var message = reader.ReadLine();

          if (!string.IsNullOrEmpty(message))
          {
            foreach (var client in clients)
            {
              StreamWriter writer = new(client.GetStream());

              if (client.GetHashCode() != tcpClient.GetHashCode())
                await writer.WriteLineAsync(tcpClient.GetHashCode() + ": " + message);
              else
              {
                await writer.WriteLineAsync("Сообщение доставлено!");
              }
              writer.Flush();
            }

            Console.WriteLine(tcpClient.GetHashCode() + ": " + message);
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      finally
      {
        DisposeClient(tcpClient);
      }
    }

    public static bool DisposeClient(TcpClient tcpClient)
    {
      if (clients.Contains(tcpClient))
      {
        tcpClient.Dispose();
        return clients.Remove(tcpClient);
      }

      return false;
    }

    public void Dispose()
    {
      _listener?.Dispose();
      _listener = null;

      foreach (var client in clients)
        client.Dispose();

      clients.Clear();
    }
  }
}