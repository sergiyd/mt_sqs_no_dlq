namespace sender;

using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
              x.UsingAmazonSqs((context, cfg) =>
                {
                  cfg.Host("eu-central-1", hostCfg =>
                  {
                    hostCfg.AccessKey(hostContext.Configuration["AccessKey"]);
                    hostCfg.SecretKey(hostContext.Configuration["SecretKey"]);
                  });
                  cfg.UseRawJsonSerializer();
                  cfg.UseDelayedMessageScheduler();


                  cfg.Message<Message>(c =>
                  {
                    c.SetEntityName("sender");
                  });

                  cfg.Publish<Message>(p =>
                  {
                    p.Durable = true;
                  });

                  cfg.ConfigureEndpoints(context);
                });
            });
            services.AddHostedService<Worker>();
          });
}
