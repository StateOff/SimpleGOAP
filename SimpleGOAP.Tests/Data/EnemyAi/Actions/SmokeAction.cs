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
        float hunger = state.GetByOwner<float>(EnemyAiFactory.KeyHunger, Owner);
        if (hunger == 0.0f)
        {
            return 1;
        }
        return Math.Max(1, 22 - 3 * state.GetByOwner<int>(EnemyAiFactory.KeyCigarettes, Owner));
    }

    public bool PreconditionMet(OwnerKeyValueState state)
    {
        float hunger = state.GetByOwner<float>(EnemyAiFactory.KeyHunger, Owner);
        return state.GetByOwner<bool>(EnemyAiFactory.KeyIsSmoker, Owner) &&
               state.GetByOwner<int>(EnemyAiFactory.KeyCigarettes, Owner) > 0 &&
               // state.GetByOwner<float>(EnemyAiFactory.KeyBoredom, Owner) > 0.8f &&
               (hunger == 0.0f || hunger > 0.4f) &&
               // Safe
               StateIsOnLocation(state) &&
               StateIsSafe(state);
    }

    public OwnerKeyValueState TakeActionOnState(OwnerKeyValueState state)
    {
        float hunger = state.GetByOwner<float>(EnemyAiFactory.KeyHunger, Owner);
        if (hunger != 0.0f)
        {
            state.SetByOwner(EnemyAiFactory.KeyHunger, Owner, MathF.Max(0.02f, hunger - 0.4f));
        }

        state.SetByOwner(EnemyAiFactory.KeyCigarettes, Owner, Math.Max(0, state.GetByOwner<int>(EnemyAiFactory.KeyCigarettes, Owner) - 1));
        return state;
    }

}