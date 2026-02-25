using TMPro;
using UnityEngine;
using Poker.Core.Models;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    public void Bind(Card card)
    {
        label.text = $"{card.Rank}\n{card.Suit}";
    }
}