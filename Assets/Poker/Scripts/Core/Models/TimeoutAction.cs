namespace Poker.Core.Models
{

    public class TimeoutAction
    {
        public Player Player { get; }

        public TimeoutAction(Player player)
        {
            Player = player;
        }
    }
}