namespace sender;

using MassTransit;
using Microsoft.Extensions.Hosting;

public class Worker : BackgroundService
{
  readonly IBus _bus;
  readonly IMessageScheduler _messageScheduler;

  public Worker(IBus bus, IMessageScheduler messageScheduler)
  {
    _bus = bus;
    _messageScheduler = messageScheduler;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {      
      Console.WriteLine($"Publish: The time is {DateTime.Now}");

      var now = DateTime.Now;
      var scheduledTime = now.AddSeconds(2);
      var message = new Message
      {
        Published = now,
        Scheduled = scheduledTime
      };

      await _messageScheduler.SchedulePublish(scheduledTime, message, stoppingToken);

      await Task.Delay(5000, stoppingToken);
    }
  }
}
