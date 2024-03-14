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
        return 1;
    }

    public bool PreconditionMet(OwnerKeyValueState state)
    {
        return state.GetByOwner<bool>(EnemyAiFactory.KeyIsThreatened, Owner) == false &&
               state.GetByOwner<float>(EnemyAiFactory.KeyAttention, Owner) <= EnemyAiFactory.AttentionState.AtEaseF;
    }

    public OwnerKeyValueState TakeActionOnState(OwnerKeyValueState state)
    {
        state.SetByOwner(EnemyAiFactory.KeyBoredom, Owner, MathF.Min(1.0f, state.GetByOwner<float>(EnemyAiFactory.KeyBoredom, Owner) + 0.1f));
        return state;
    }

}