using Server;
using System.Net;

namespace Seminar1;

internal class Program
{
  static void Main()
  {
    CancellationTokenSource cts = new CancellationTokenSource();
    CancellationToken ct = cts.Token;

    IPEndPoint iPEndPoint = new(IPAddress.Any, 55555);
    using ChatServer cs = new(iPEndPoint);

    cs.Run(ct);

    string? serverMessage = string.Empty;
    while (!serverMessage.Equals("exit", StringComparison.InvariantCultureIgnoreCase) &&
           !serverMessage.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
    {
      serverMessage = Console.ReadLine();
    }

    try
    {
      cts.Cancel();
    }
    finally
    {
      Console.WriteLine("Сервер прекращает свою работу!");
      cs.Dispose();
    }
  }
}