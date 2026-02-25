using Poker.Core.Enums;
using System.Collections.Generic;

namespace Poker.Core.Models
{

    public class EvaluatedHand
    {
        public Player Player { get; }
        public HandRank Rank { get; }
        public int Score { get; }

        public EvaluatedHand(Player player, HandRank rank, int score)
        {
            Player = player;
            Rank = rank;
            Score = score;
        }
    }
}