using Poker.Core.Models;
using Poker.Core.Enums;
using System.Collections.Generic;
using System.Linq;

public class HandEvaluator
{
    public EvaluatedHand Evaluate(Player player, List<Card> hole, List<Card> community)
    {
        var cards = hole.Concat(community).ToList();

        var groups = cards
            .GroupBy(c => c.Rank)
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => g.Key)
            .ToList();

        int maxCount = groups.First().Count();

        if (maxCount == 3)
            return Build(player, HandRank.ThreeOfKind, groups);

        if (maxCount == 2)
        {
            if (groups.Count(g => g.Count() == 2) >= 2)
                return Build(player, HandRank.TwoPair, groups);

            return Build(player, HandRank.Pair, groups);
        }

        return Build(player, HandRank.HighCard, groups);
    }

    private EvaluatedHand Build(Player player, HandRank rank, List<IGrouping<Rank, Card>> groups)
    {
        int score = (int)rank * 100 + (int)groups.First().Key;
        return new EvaluatedHand(player, rank, score);
    }
    
    public Player DetermineWinner(GameSnapshot snapshot)
    {
        var results = new List<EvaluatedHand>();

        foreach (var player in snapshot.Players)
        {
            var hand = Evaluate(
                player,
                snapshot.PlayerHands[player.Id],
                snapshot.CommunityCards
            );

            results.Add(hand);
        }

        return results
            .OrderByDescending(r => r.Score)
            .First()
            .Player;
    }
}