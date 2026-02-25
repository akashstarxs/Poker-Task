using Poker.Core.Models;
using System.Collections.Generic;

public class PotService
{
    private readonly Dictionary<string, int> _contributions = new();

    public int Pot { get; private set; }

    public void PlaceBet(Player player, int amount)
    {
        if (amount <= 0) return;

        player.Deduct(amount);
        Pot += amount;

        if (_contributions.ContainsKey(player.Id))
            _contributions[player.Id] += amount;
        else
            _contributions[player.Id] = amount;
    }

    public void Reset()
    {
        Pot = 0;
        _contributions.Clear();
    }

    public void DistributeToWinner(Player winner)
    {
        winner.Add(Pot);
        Reset();
    }
}