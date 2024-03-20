using System;
using System.Linq;
using SimpleGOAP;
using SimpleGOAP.KeyValueState;

namespace asgae.Ai.Actions;

public class GotoAction : OwnerActionBase, IAction<OwnerKeyValueState>
{

    private int[] FromLocations;
    private int ToLocation;
    
    public GotoAction(string owner, int[] fromLocations, int toLocation) : base(owner)
    {
        FromLocations = fromLocations;
        ToLocation = toLocation;
    }
    
    public string Title => $"GotoAction({String.Join('|', FromLocations)} to {ToLocation}) by {Owner}";

    public int GetCost(OwnerKeyValueState state)
    {
        // FIXME: Distance
        return 1;
    }

    public bool PreconditionMet(OwnerKeyValueState state)
    {
        return FromLocations.Contains(state.GetByOwner<int>(EnemyAiFactory.Location, Owner)) &&
               StateIsSafe(state);
    }

    public OwnerKeyValueState TakeActionOnState(OwnerKeyValueState state)
    {
        state.SetByOwner(EnemyAiFactory.Location, Owner, ToLocation);
        return state;
    }

}