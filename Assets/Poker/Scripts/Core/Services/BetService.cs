using Poker.Core.Models;

public class BetService
{
    private readonly PotService _potService;

    public BetService(PotService potService)
    {
        _potService = potService;
    }

    public void ProcessAction(PlayerAction action)
    {
        switch (action.Type)
        {
            case ActionType.Bet:
            case ActionType.Call:
                _potService.PlaceBet(action.Player, action.Amount);
                break;

            case ActionType.Fold:
                // mark folded later (optional)
                break;

            case ActionType.Timeout:
                // treat as check/fold depending rules
                break;
        }
    }
}