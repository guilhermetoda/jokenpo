using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jokenpo
{
    public class GameRoomGrain : Grain, IGameRoom
    {
        // IDLE, Created after 1 player connects
        // WAITING_FOR_PLAYERS - waiting for two players to connect
        // WAITING_FOR_2_MOVES - waiting for two players to make a move
        // WAITING_FOR_1_MOVE - waiting for two players to make a move
        // ENDED - Game finished

        GameStatus gameStatus;
        List<IPlayer> playersConnected;
        //List<Moves> playersMoves;
        Dictionary<Guid, Moves> playersMoves;
        
        Guid winner = Guid.Empty;

        public override Task OnActivateAsync()
        {
            CleanRoom();
            return base.OnActivateAsync();
        }


        private void CleanRoom()
        {
            Console.WriteLine("Cleaning Room");
            playersConnected = new List<IPlayer>();
            playersMoves = new Dictionary<Guid, Moves>();
            gameStatus = GameStatus.IDLE;
        }

        Task<GameStatus> IGameRoom.GetGameStatus() 
        {
            Console.WriteLine(gameStatus);
            return Task.FromResult(gameStatus);
        }

        Task<bool> IGameRoom.ConnectPlayer(IPlayer player, Guid playerId) 
        {
            if (playersConnected.Count > 1) 
            {
                return Task.FromResult(false);
            }
            playersConnected.Add(player);
            playersMoves.Add(playerId, Moves.UNSET);
            Console.WriteLine("Adding player to the room");
            if (playersConnected.Count == 2) 
            {
                Console.WriteLine("two players in the room");
                gameStatus = GameStatus.WAITING_FOR_2_MOVES;
                playersConnected[0].SetGame(true);

                playersConnected[1].SetGame(true);
            }
            Console.WriteLine("Player added");

            return Task.FromResult(true);
        }

        Task<bool> IGameRoom.IsAvailable() 
        {
            if (playersConnected.Count < 2) 
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        Task IGameRoom.Play() 
        {
            Console.WriteLine("Play");
            return Task.CompletedTask;
        }

        Task IGameRoom.SetPlayerMove(Player player, Moves playerMoves) 
        {
            playersMoves[player.Id] = playerMoves;
            if (gameStatus == GameStatus.WAITING_FOR_2_MOVES) 
            {
                gameStatus = GameStatus.WAITING_FOR_1_MOVE;
            }
            else if (gameStatus == GameStatus.WAITING_FOR_1_MOVE) 
            {
                Console.WriteLine("Checking Winner...");
                winner = CheckWinner();
                gameStatus = GameStatus.ENDED;
                playersConnected[0].SetFinished(winner);
                playersConnected[1].SetFinished(winner);
            }
            Console.WriteLine("Move Set");
            return Task.CompletedTask;
        }

        private Guid CheckWinner() 
        {
            List<Guid> playersIdList = new List<Guid>();
            foreach(var item in playersMoves)
            {
                playersIdList.Add(item.Key);
            }

            if ((playersConnected.Count > 1) && (playersMoves.Count > 1))
            {
                if (playersMoves[playersIdList[0]] == playersMoves[playersIdList[1]]) 
                {
                    return Guid.Empty;
                }
                else if (playersMoves[playersIdList[0]] == Moves.PAPER) 
                {
                    if (playersMoves[playersIdList[1]] == Moves.ROCK) 
                    {
                        return playersIdList[0];
                    }
                    else 
                    {
                        //player 1 win
                        return playersIdList[1];
                    }
                }
                else if (playersMoves[playersIdList[0]] == Moves.ROCK) 
                {
                    if (playersMoves[playersIdList[1]] == Moves.SCISSORS) 
                    {
                        //player 0 win
                        return playersIdList[0];
                    }
                    else 
                    {
                        //player 1 win
                        return playersIdList[1];
                    }
                }
                else if (playersMoves[playersIdList[0]] == Moves.SCISSORS) 
                {
                    if (playersMoves[playersIdList[1]] == Moves.PAPER) 
                    {
                        //player 0 win
                        return playersIdList[0];
                    }
                    else 
                    {
                        //player 1 win
                        return playersIdList[1];
                    }
                }
            }
            //error
            return Guid.Empty;
        }

        Task<bool> IGameRoom.IsPlayerWinner(Player player) 
        {
            if (winner != Guid.Empty) 
            {
                if (player.Id == winner) 
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }
    }
}