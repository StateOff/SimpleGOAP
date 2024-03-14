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
        return 2;
    }

    public bool PreconditionMet(OwnerKeyValueState state)
    {
        return state.GetByOwner<float>(EnemyAiFactory.KeyBoredom, Owner) > 0.4f &&
               // Safe
               state.GetByOwner<bool>(EnemyAiFactory.KeyIsThreatened, Owner) == false &&
               state.GetByOwner<float>(EnemyAiFactory.KeyAttention, Owner) <= EnemyAiFactory.AttentionState.AtEaseF;
    }

    public OwnerKeyValueState TakeActionOnState(OwnerKeyValueState state)
    {
        state.SetByOwner(EnemyAiFactory.KeyBoredom, Owner, MathF.Max(0.0f, state.GetByOwner<float>(EnemyAiFactory.KeyBoredom, Owner) - 0.2f));
        return state;
    }

}