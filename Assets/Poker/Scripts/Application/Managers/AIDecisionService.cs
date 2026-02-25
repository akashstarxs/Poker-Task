using Poker.Core.Models;
using Poker.Core.Enums;
using System;

public class AIDecisionService
{
    private readonly HandEvaluator _evaluator;
    private readonly Random _rng = new();

    public AIDecisionService(HandEvaluator evaluator)
    {
        _evaluator = evaluator;
    }

    public PlayerAction Decide(GameSnapshot snapshot, Player player)
    {
        var evaluated = _evaluator.Evaluate(
            player,
            snapshot.PlayerHands[player.Id],
            snapshot.CommunityCards
        );

        int strength = (int)evaluated.Rank;

        // Simple decision rules
        if (strength >= (int)HandRank.ThreeOfKind)
            return Bet(player, snapshot);

        if (strength >= (int)HandRank.Pair)
            return Call(player, snapshot);

        // Weak hand → random fold/check
        return _rng.NextDouble() < 0.5
            ? new PlayerAction(player, ActionType.Fold)
            : new PlayerAction(player, ActionType.Check);
    }

    private PlayerAction Bet(Player player, GameSnapshot snapshot)
    {
        int amount = Math.Min(50, player.Chips);
        return new PlayerAction(player, ActionType.Bet, amount);
    }

    private PlayerAction Call(Player player, GameSnapshot snapshot)
    {
        int amount = Math.Min(20, player.Chips);
        return new PlayerAction(player, ActionType.Call, amount);
    }
}