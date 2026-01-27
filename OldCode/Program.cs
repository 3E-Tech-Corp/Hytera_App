using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting; 

namespace FunTimePIE
{
    public class Program
    {
        public static  void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

           // var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
           //?? "your-openai-api-key-here";

           // var agent = new VoiceInventoryAgent(apiKey);

           // agent.StartListening();

           // Console.WriteLine("Voice Inventory Agent Running...");
           // Console.WriteLine("Say 'check inventory' followed by a product name");
           // Console.WriteLine("Press 'q' to quit");

           // while (Console.ReadKey().KeyChar != 'q')
           // {
           //     await Task.Delay(100);
           // }

           // agent.StopListening();
           // agent.Dispose();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
