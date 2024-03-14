using System;
using SimpleGOAP;
using SimpleGOAP.KeyValueState;

namespace asgae.Ai.Actions;

public class SmokeAction : OwnerActionBase, IAction<OwnerKeyValueState> {
    
    public SmokeAction(string owner) : base(owner)
    {
    }
    
    public string Title => $"SmokeAction by {Owner}";

    public int GetCost(OwnerKeyValueState state)
    {
        return 1;
    }

    public bool PreconditionMet(OwnerKeyValueState state)
    {
        return state.GetByOwner<bool>(EnemyAiFactory.KeyIsSmoker, Owner) &&
               state.GetByOwner<int>(EnemyAiFactory.KeyCigarettes, Owner) > 0 &&
               // state.GetByOwner<float>(EnemyAiFactory.KeyBoredom, Owner) > 0.8f &&
               state.GetByOwner<float>(EnemyAiFactory.KeyHunger, Owner) > 0.4f &&
               // Safe
               state.GetByOwner<bool>(EnemyAiFactory.KeyIsThreatened, Owner) == false &&
               state.GetByOwner<float>(EnemyAiFactory.KeyAttention, Owner) <= EnemyAiFactory.AttentionState.AtEaseF;
    }

    public OwnerKeyValueState TakeActionOnState(OwnerKeyValueState state)
    {
        state.SetByOwner(EnemyAiFactory.KeyBoredom, Owner, MathF.Max(0.0f, state.GetByOwner<float>(EnemyAiFactory.KeyBoredom, Owner) - 0.3f));
        state.SetByOwner(EnemyAiFactory.KeyHunger, Owner, MathF.Max(0.0f, state.GetByOwner<float>(EnemyAiFactory.KeyHunger, Owner) - 0.4f));
        state.SetByOwner(EnemyAiFactory.KeyCigarettes, Owner, Math.Max(0, state.GetByOwner<int>(EnemyAiFactory.KeyCigarettes, Owner) - 1));
        return state;
    }

}