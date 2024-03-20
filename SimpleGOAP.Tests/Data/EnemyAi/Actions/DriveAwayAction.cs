using System;
using SimpleGOAP;
using SimpleGOAP.KeyValueState;

namespace asgae.Ai.Actions;

public class DriveAwayAction : OwnerActionBase, IAction<OwnerKeyValueState> {
    
    public DriveAwayAction(string owner) : base(owner)
    {
    }
    
    public string Title => $"DriveAwayAction by {Owner}";

    public int GetCost(OwnerKeyValueState state)
    {
        return 100;
    }

    public bool PreconditionMet(OwnerKeyValueState state)
    {
        return state.GetByOwner<int>(EnemyAiFactory.Location, Owner) == 1 &&
               state.GetByOwner<int>(EnemyAiFactory.KeyFood, Owner) == 0 &&
               state.GetByOwner<float>(EnemyAiFactory.KeyHunger, Owner) > 0.9f &&
               // Safe
               StateIsSafe(state);
    }

    public OwnerKeyValueState TakeActionOnState(OwnerKeyValueState state)
    {
        state.SetByOwner(EnemyAiFactory.KeyBoredom, Owner, 0.0f);
        state.SetByOwner(EnemyAiFactory.KeyHunger, Owner, 0.0f);
        state.SetByOwner(EnemyAiFactory.Location, Owner, 2);
        return state;
    }

}