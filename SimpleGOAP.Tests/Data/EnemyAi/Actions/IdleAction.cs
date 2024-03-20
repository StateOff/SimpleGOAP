using System;
using SimpleGOAP;
using SimpleGOAP.KeyValueState;

namespace asgae.Ai.Actions;

public class IdleAction : OwnerActionBase, IAction<OwnerKeyValueState> {
    
    public IdleAction(string owner) : base(owner)
    {
    }
    
    public string Title => $"IdleAction by {Owner}";

    public int GetCost(OwnerKeyValueState state)
    {
        return 2;
    }

    public bool PreconditionMet(OwnerKeyValueState state)
    {
        return StateIsOnLocation(state) &&
               StateIsSafe(state);
    }

    public OwnerKeyValueState TakeActionOnState(OwnerKeyValueState state)
    {
        state.SetByOwner(EnemyAiFactory.KeyBoredom, Owner, MathF.Min(1.0f, state.GetByOwner<float>(EnemyAiFactory.KeyBoredom, Owner) + 0.2f));
        return state;
    }
}