namespace receiver;

using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using sender;


public class Program
{
  public static void Main(string[] args)
  {
    CreateHostBuilder(args)
      .ConfigureAppConfiguration((_, config) =>
      {
        config
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
      }).Build().Run();
  }

  public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
      .ConfigureServices((hostContext, services) =>
      {
        services.AddMassTransit(x =>
          {
            x.AddDelayedMessageScheduler();

            x.AddConsumer<MessageConsumer>();

            x.UsingAmazonSqs((context, cfg) =>
              {
                cfg.Host("eu-central-1", hostCfg =>
                  {
                    hostCfg.AccessKey(hostContext.Configuration["AccessKey"]);
                    hostCfg.SecretKey(hostContext.Configuration["SecretKey"]);
                  });

                cfg.UseRawJsonSerializer();
                cfg.UseDelayedMessageScheduler();
                cfg.Message<Message>(c => c.SetEntityName("sender"));

                cfg.ReceiveEndpoint("sender-receiver", configure =>
                  {
                    configure.Subscribe("sender", s =>
                      {
                        s.Durable = true;
                      });

                    configure.ConfigureConsumer<MessageConsumer>(context);
                  });
                cfg.ConfigureEndpoints(context);
              });
          });
      });
}
