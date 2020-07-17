using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Threading.Tasks;

namespace Jokenpo
{
    public class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    Console.WriteLine(@"                    
     ██  ██████  ██   ██ ███████ ███    ██ ██████   ██████  
     ██ ██    ██ ██  ██  ██      ████   ██ ██   ██ ██    ██ 
     ██ ██    ██ █████   █████   ██ ██  ██ ██████  ██    ██ 
██   ██ ██    ██ ██  ██  ██      ██  ██ ██ ██      ██    ██ 
 █████   ██████  ██   ██ ███████ ██   ████ ██       ██████  
 
                    ");
                    await DoClientWork(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> ConnectClient()
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "Jokenpo";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGameRoom).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        public static async Task<IGameRoom> GetRoom(IClusterClient client) 
        {
            bool roomFound = false;
            int i = 0;
            IGameRoom room = null;
            while (!roomFound) 
            {
                room = client.GetGrain<IGameRoom>(i);
                bool roomAvailable = await room.IsAvailable();
                if (!roomAvailable) 
                {
                    i++;
                }
                else 
                {
                    Console.WriteLine($"Connected on Room #{i} \n");
                    roomFound = true;
                }
            }
            return room;
        }


        private static async Task DoClientWork(IClusterClient client)
        {
            Console.WriteLine("Let's Play Rock, Paper & Scissor!");
            Console.WriteLine("What's your name? ");
            string name = Console.ReadLine();
            
            while (name == "") 
            {
                Console.WriteLine("unfortunately you can't play anonymous, please type your name");
                name = Console.ReadLine();
            }
            
            var player = client.GetGrain<IPlayer>(Guid.NewGuid());
            
            var room = GetRoom(client).Result;
            Console.WriteLine("Setting room");
            player.SetRoom(room).Wait();
            
            await player.SetName(name);

            bool canPlayGame = player.CanPlayGame().Result;
            while (!canPlayGame) 
            {
                Console.WriteLine("Please wait until we find you a challenger...");
                await Task.Delay(TimeSpan.FromSeconds(3));
                canPlayGame = player.CanPlayGame().Result;
            }
            
            bool correctMove = false;
            Moves playerMove = Moves.PAPER;
            while (!correctMove) 
            {
                Console.WriteLine("What's your move? Type 1 for Rock, 2 for Paper or 3 for Scissors");
                var move = int.Parse(Console.ReadLine());
                if (Enum.IsDefined(typeof(Moves), move)) 
                {  
                    correctMove = true;
                    playerMove = (Moves)move;

                }
                else 
                {
                    Console.WriteLine("Please, try again... ");
                }
            }

            await player.Play(playerMove);
            GameMoves.MovesDraw(playerMove);

            bool hasWinner = false;
            while (!hasWinner) 
            {
                Console.WriteLine("Waiting for the other player move");
                await Task.Delay(TimeSpan.FromSeconds(3));
                hasWinner = player.IsGameFinished().Result;
                
            }

            if (player.IsWinner().Result) 
            {
                Console.WriteLine("YOU WIN!!!");
            }
            else 
            {
                Console.WriteLine("You Lose");
            }


        }
    }

}