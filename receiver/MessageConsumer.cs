namespace receiver;

using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using sender;

public class MessageConsumer :
    IConsumer<Message>
{
  readonly ILogger<MessageConsumer> _logger;

  public MessageConsumer(ILogger<MessageConsumer> logger)
  {
    _logger = logger;
  }

  public Task Consume(ConsumeContext<Message> context)
  {
    var now = DateTime.Now;

    _logger.LogInformation(
      string.Join(
        Environment.NewLine,
        "===============================================================",
        $"Received: {now}, delta (now - scheduled): {now - context.Message.Scheduled}",
        $"Published: {context.Message.Published}, delta (now - published): {now - context.Message.Published}",
        $"Scheduled: {context.Message.Scheduled}," +
          $" delta (scheduled - published): {context.Message.Scheduled - context.Message.Published}",
        "==============================================================="
      )
    );

    return Task.CompletedTask;
  }
}
