
using Poker.Core.Enums;
using System.Collections.Generic;

namespace Poker.Core.Models
{
    public class GameSnapshot
    {
        public List<Player> Players = new();
        public int Pot;
        public int CurrentPlayerIndex;
        public PokerRound Round;
        public Dictionary<string, List<Card>> PlayerHands = new();
        public List<Card> CommunityCards = new();
    }
}
