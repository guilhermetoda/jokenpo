using Orleans;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Jokenpo
{
    public enum GameStatus 
        {
            IDLE,
            WAITING_FOR_PLAYERS,
            WAITING_FOR_2_MOVES,
            WAITING_FOR_1_MOVE,
            ENDED
        }

    public interface IGameRoom : IGrainWithIntegerKey
    {
        Task<bool> ConnectPlayer(IPlayer player, Guid playerId);
        Task<bool> IsAvailable();
        Task SetPlayerMove(Player player, Moves playerMoves);
        Task<bool> IsPlayerWinner(Player player);
        Task Play();

        Task<GameStatus> GetGameStatus();
    }
}