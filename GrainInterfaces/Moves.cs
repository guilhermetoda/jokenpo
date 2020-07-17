using Orleans.Concurrency;
using System;

namespace Jokenpo
{    
    public enum Moves
    {
            ROCK,
            PAPER,
            SCISSORS,
            UNSET
    }
    public class GameMoves 
    {
        public static void MovesDraw(Moves move) 
        {
            switch (move) 
            {
                case Moves.ROCK:
                    Console.WriteLine(@"
    _______
---'   ____)
      (_____)
      (_____)
      (____)
---.__(___)
                ");
                    break;
                case Moves.PAPER:
                    Console.WriteLine(@"
     _______
---'    ____)____
           ______)
          _______)
         _______)
---.__________)
                ");
                    break;
                case Moves.SCISSORS:
                    Console.WriteLine(@"
    _______
---'   ____)____
          ______)
       __________)
      (____)
---.__(___)
                ");
                break;
            }

        }
    }
}
