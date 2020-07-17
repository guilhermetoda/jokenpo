using Orleans;
using System;
using System.Threading.Tasks;

namespace Jokenpo
{
    public interface IPlayer : IGrainWithGuidKey
    {
        Task SetName(string name);
        Task SetRoom(IGameRoom room);
        Task Play(Moves playerMove);
        Task SetGame(bool isPlayable);
        
        Task<bool> CanPlayGame();
        Task<Player> GetPlayer();
        Task<bool> IsWinner();
        Task<bool> IsGameFinished();
        Task SetFinished(Guid winnerId);

    }
}