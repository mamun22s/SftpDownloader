using ActiveXTService;
using ActiveXTService.Data;
using ActiveXTService.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx,services) =>
    {
        services.AddHostedService<Worker>();
        //DB context
        services.AddDbContext<SftpDataContext>(options =>
        options.UseNpgsql(ctx.Configuration.GetConnectionString("SftpDb")));
        //PG timestamp with c# Datetime compatibility
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        //Appsettings
        services.Configure<SftpSettingsModel>(ctx.Configuration.GetSection("SftpSettings"));
        //Serilog
    }).UseSerilog((hostingContext, loggerConfig) =>
                                   loggerConfig.ReadFrom.Configuration(hostingContext.Configuration)
                                    )
                                    .Build();

await host.RunAsync();
