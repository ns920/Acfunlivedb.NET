using AcfunApi;
using Acfunlivedb.NET;
using Acfunlivedb.NET.DAL;
using Acfunlivedb.NET.Services;
using Acfunlivedb.NET.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class Program
{
    private readonly IConfiguration _configuration;

    private static readonly string HELP = @"指令列表：
u <name> 按主播名字搜索直播
uall <name> 按主播名字搜索所有直播
uid <uid> 按主播uid搜索直播
uidall <uid> 按主播uid搜索所有直播
getplayback <liveid> 获取直播回放
";

    public Program(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private static async Task Main(string[] args)
    {
        if (!File.Exists("appsettings.json"))
        {
            File.WriteAllText("appsettings.json", """
                                {
                  "ConnectionStrings": {
                    "sqlite": "Data Source=./acfunlive.db;"
                  },
                  "Queryinterval": 75000
                }
                """);
            Thread.Sleep(3000);
        }

        IConfiguration Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddCommandLine(args)
            .Build();
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<AcfunlivedbDbContext>((options) =>
                {
                    options.UseSqlite(Configuration.GetConnectionString("sqlite"));
                });
                services.AddScoped<AutoMigrations>();
                services.AddScoped<GetLiveListService>();
                services.AddHostedService<RequestApiWorker>();
                services.AddScoped<SearchService>();
                services.AddScoped<GetPlaybackServices>();
                services.AddSingleton<Request>();
            })
            .ConfigureLogging(
            logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
            }
            ).Build();
        StartServices(host.Services);
        await host.StartAsync();
        string? command = "";
        while ((command = Console.ReadLine()) != "exit")
        {
            try
            {
                string[] commands = command.Split(" ");
                using IServiceScope serviceScope = host.Services.CreateScope();
                IServiceProvider provider = serviceScope.ServiceProvider;
                SearchService searchService = provider.GetRequiredService<SearchService>();
                GetPlaybackServices getPlaybackServices = provider.GetRequiredService<GetPlaybackServices>();
                _ = commands[0] switch
                {
                    "u" => WriteStrings(searchService.GetLiveStringByNameDefault(commands[1])),
                    "uall" => WriteStrings(searchService.GetLiveStringByName(commands[1])),
                    "uid" => WriteStrings(searchService.GetLiveStringByUidDefault(commands[1])),
                    "uidall" => WriteStrings(searchService.GetLiveStringByUid(commands[1])),
                    "getplayback" => WriteStrings(await getPlaybackServices.Get(commands[1])),
                    _ => throw new Exception("命令错误"),
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(HELP);
            }
        }
    }

    private static void StartServices(IServiceProvider hostProvider)
    {
        //DI获取迁移类
        using IServiceScope serviceScope = hostProvider.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;
        AutoMigrations autoMigrations = provider.GetRequiredService<AutoMigrations>();
        //自动执行迁移
        autoMigrations.CommitMigrations();
    }

    private static int WriteStrings(List<string> strings)
    {
        foreach (string s in strings)
        {
            Console.WriteLine(s);
        }
        return 0;
    }
}