using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Jokenpo
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine(@"
                    

     ██  ██████  ██   ██ ███████ ███    ██ ██████   ██████  
     ██ ██    ██ ██  ██  ██      ████   ██ ██   ██ ██    ██ 
     ██ ██    ██ █████   █████   ██ ██  ██ ██████  ██    ██ 
██   ██ ██    ██ ██  ██  ██      ██  ██ ██ ██      ██    ██ 
 █████   ██████  ██   ██ ███████ ██   ████ ██       ██████  
                                                            
                                                            
                    

                    ");
                Console.WriteLine("The Server!");
                //Console.WriteLine("\n\n Press Enter to terminate... \n\n");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "Jokenpo";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(GameRoomGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            Console.WriteLine("\nStart Silo\n");
            await host.StartAsync();
            return host;
        }
    }
}