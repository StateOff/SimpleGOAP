using System;
using SimpleGOAP;
using SimpleGOAP.KeyValueState;

namespace asgae.Ai.Actions;

public class EatFoodAction : OwnerActionBase, IAction<OwnerKeyValueState> {
    
    public EatFoodAction(string owner) : base(owner)
    {
    }
    
    public string Title => $"EatFoodAction by {Owner}";

    public int GetCost(OwnerKeyValueState state)
    {
        return 13;
    }

    public bool PreconditionMet(OwnerKeyValueState state)
    {
        return state.GetByOwner<int>(EnemyAiFactory.KeyFood, Owner) > 0 &&
               state.GetByOwner<float>(EnemyAiFactory.KeyHunger, Owner) > 0.9f &&
               
               StateIsOnLocation(state) &&
               StateIsSafe(state);
    }

    public OwnerKeyValueState TakeActionOnState(OwnerKeyValueState state)
    {
        state.SetByOwner(EnemyAiFactory.KeyBoredom, Owner, MathF.Max(0.0f, state.GetByOwner<float>(EnemyAiFactory.KeyBoredom, Owner) - 0.4f));
        state.SetByOwner(EnemyAiFactory.KeyFood, Owner, Math.Max(0, state.GetByOwner<int>(EnemyAiFactory.KeyFood, Owner) - 1));
        state.SetByOwner(EnemyAiFactory.KeyHunger, Owner, 0.0f);
        return state;
    }

}