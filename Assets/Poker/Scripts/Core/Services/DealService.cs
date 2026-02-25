using Poker.Core.Models;
using System.Collections.Generic;

public class DealService
{
    private readonly DeckService _deck;

    public DealService(DeckService deck)
    {
        _deck = deck;
    }

    public void DealHoleCards(GameSnapshot snapshot)
    {
        snapshot.PlayerHands.Clear();

        foreach (var player in snapshot.Players)
        {
            snapshot.PlayerHands[player.Id] = new List<Card>
            {
                _deck.Draw(),
                _deck.Draw()
            };
        }
    }

    public void DealFlop(GameSnapshot snapshot)
    {
        snapshot.CommunityCards.Add(_deck.Draw());
        snapshot.CommunityCards.Add(_deck.Draw());
        snapshot.CommunityCards.Add(_deck.Draw());
    }

    public void DealTurn(GameSnapshot snapshot)
    {
        snapshot.CommunityCards.Add(_deck.Draw());
    }

    public void DealRiver(GameSnapshot snapshot)
    {
        snapshot.CommunityCards.Add(_deck.Draw());
    }
}