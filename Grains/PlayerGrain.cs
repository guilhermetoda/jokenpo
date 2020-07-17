using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jokenpo
{
    public class PlayerGrain : Orleans.Grain, IPlayer
    {
        public Player player;
        public bool canPlay = false;

        bool isWinner = false;
        bool isGameFinished = false;
        int indexInRoom = -1;
        
        IGameRoom currentRoom; 
        
        public override Task OnActivateAsync()
        {
            player = new Player { Id = this.GetPrimaryKey(), Name = "nobody", MatchesPlayed = 0, GamesWon = 0, GamesLost = 0};
            return base.OnActivateAsync();
        }

        Task IPlayer.SetName(string name) 
        {
            Console.WriteLine($"Setting the name for {name}");
            player.Name = name;
            return Task.CompletedTask;
        }

        Task IPlayer.SetRoom(IGameRoom room) 
        {
            Console.WriteLine("Connecting Player...");
            room.ConnectPlayer(this, this.player.Id);
            currentRoom = room; 
            Console.WriteLine("Player Connected");
            return Task.CompletedTask;
        }

        Task IPlayer.Play(Moves playerMove) 
        {
            currentRoom.SetPlayerMove(player, playerMove);
            return Task.CompletedTask;
        }

        Task<bool> IPlayer.CanPlayGame()
        {
            return Task.FromResult(canPlay);
        }

        Task IPlayer.SetGame(bool isPlayable)  
        {
            canPlay = isPlayable;
            return Task.CompletedTask;
        }

        Task IPlayer.SetFinished(Guid winnerId)  
        {
            isGameFinished = true;
            if (winnerId == this.player.Id) 
            {
                isWinner = true;
            }
            return Task.CompletedTask;
        }

        Task<Player> IPlayer.GetPlayer() 
        {
            return Task.FromResult(this.player);
        }

        Task<bool> IPlayer.IsWinner() 
        {
            return Task.FromResult(isWinner);
        }

        Task<bool> IPlayer.IsGameFinished() 
        {
            return Task.FromResult(isGameFinished);
        }

    }
}