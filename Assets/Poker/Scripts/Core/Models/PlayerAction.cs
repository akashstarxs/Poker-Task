namespace Poker.Core.Models
{

    public enum ActionType
    {
        Bet,
        Call,
        Fold,
        Check,
        Timeout
    }

    public class PlayerAction
    {
        public Player Player { get; }
        public ActionType Type { get; }
        public int Amount { get; }

        public PlayerAction(Player player, ActionType type, int amount = 0)
        {
            Player = player;
            Type = type;
            Amount = amount;
        }
    }
}