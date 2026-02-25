using Poker.Core.Models;
using System;
using System.Collections.Generic;

public class DeckService
{
    private readonly List<Card> _deck = new();
    private readonly Random _rng = new();

    public void Initialize()
    {
        _deck.Clear();

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            _deck.Add(new Card(suit, rank));

        Shuffle();
    }

    public Card Draw()
    {
        var card = _deck[0];
        _deck.RemoveAt(0);
        return card;
    }

    private void Shuffle()
    {
        for (int i = _deck.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(i + 1);
            (_deck[i], _deck[j]) = (_deck[j], _deck[i]);
        }
    }
}