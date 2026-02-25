namespace Poker.Core.Models
{

    public class Player
    {
        public string Id { get; }
        public string Name { get; }
        public int Chips { get; private set; }
        public bool IsAI { get; }

        public Player(string id, string name, int chips, bool isAI)
        {
            Id = id;
            Name = name;
            Chips = chips;
            IsAI = isAI;
        }

        public void Deduct(int amount) => Chips -= amount;
        public void Add(int amount) => Chips += amount;
    }
}