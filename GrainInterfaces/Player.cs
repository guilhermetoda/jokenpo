using Orleans.Concurrency;
using System;

namespace Jokenpo
{
    [Immutable]
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int MatchesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
    }
}