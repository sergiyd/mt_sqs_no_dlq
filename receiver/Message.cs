namespace sender;

using MassTransit;

public class Message : QueuePayload
{ }

[ExcludeFromTopology]
public abstract class QueuePayload
{
  public DateTime Published { get; init; }
  public DateTime Scheduled { get; init; }
}
