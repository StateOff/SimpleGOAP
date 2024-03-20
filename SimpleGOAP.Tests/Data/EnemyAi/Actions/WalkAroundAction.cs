using System;
using SimpleGOAP;
using SimpleGOAP.KeyValueState;

namespace asgae.Ai.Actions;

public class WalkAroundAction : OwnerActionBase, IAction<OwnerKeyValueState> {
    
    public WalkAroundAction(string owner) : base(owner)
    {
    }
    
    public string Title => $"WalkingAroundAction by {Owner}";

    public int GetCost(OwnerKeyValueState state)
    {
        return 1;
    }

    public bool PreconditionMet(OwnerKeyValueState state)
    {
        return state.GetByOwner<float>(EnemyAiFactory.KeyBoredom, Owner) > 0.4f &&
               
               StateIsOnLocation(state) &&
               StateIsSafe(state);
    }

    public OwnerKeyValueState TakeActionOnState(OwnerKeyValueState state)
    {
        state.SetByOwner(EnemyAiFactory.KeyBoredom, Owner, MathF.Max(0.0f, state.GetByOwner<float>(EnemyAiFactory.KeyBoredom, Owner) - 0.2f));
        state.SetByOwner(EnemyAiFactory.KeyHunger, Owner, MathF.Max(0.0f, state.GetByOwner<float>(EnemyAiFactory.KeyHunger, Owner) + 0.1f));
        return state;
    }

}