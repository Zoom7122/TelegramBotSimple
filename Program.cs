using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBotSimple.Controllers;
using VoiceTexterBot.Controllers;

namespace SimpleBot
{
    static class Program
    {
        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.Unicode;

            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) => ConfigureServices(services))
                .UseConsoleLifetime()
                .Build();

            Console.WriteLine("Starting Service");
            await host.RunAsync();
            Console.WriteLine("Service stopped");
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient("Token"));
            services.AddHostedService<Bot>();
            services.AddTransient<TextMessageController>();
            services.AddTransient<InlineKeyboardController>();
        }
    }
}
