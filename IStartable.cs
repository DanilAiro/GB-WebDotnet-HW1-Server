namespace Server;
internal interface IStartable:IDisposable
{
  public Task Run(CancellationToken ct);
}
