using System;
using System.Linq;
using SimpleGOAP;
using Xunit.Sdk;

namespace asgae.Ai.Actions;

public class SwapStateAtLocationAction<T> : OwnerActionBase, IAction<OwnerKeyValueState>
{
    private readonly string fromOwner;

    private readonly string key;
    // FIXME: net7.0 has Generics for INumber
    private readonly dynamic value;
    private readonly bool onlyIfOwnerHasNone;
    private readonly int[] location;

    public SwapStateAtLocationAction(string fromOwner, string owner, string key, T value, bool onlyIfOwnerHasNone = false, int[] location = null) : base(owner)
    {
        this.fromOwner = fromOwner;
        this.key = key;
        this.value = value;
        this.onlyIfOwnerHasNone = onlyIfOwnerHasNone;
        this.location = location;
    }
    
    public string Title => $"SwapStateAtLocationAction by {Owner}";

    public int GetCost(OwnerKeyValueState state)
    {
        return 10;
    }

    public bool PreconditionMet(OwnerKeyValueState state)
    {
        var fromOwnerValue = state.GetByOwner<T>(key, fromOwner);
        dynamic oldValue = state.GetByOwner<T>(key, Owner);
        return (location == null || location.Contains(state.GetByOwner<int>(EnemyAiFactory.Location, Owner))) &&
               (!onlyIfOwnerHasNone || oldValue == 0) &&
               fromOwnerValue >= value &&
               StateIsSafe(state);
    }

    public OwnerKeyValueState TakeActionOnState(OwnerKeyValueState state)
    {
        var fromOwnerValue = state.GetByOwner<T>(key, fromOwner);
        dynamic oldValue = state.GetByOwner<T>(key, Owner);
        state.SetByOwner(key, fromOwner, fromOwnerValue - value);
        state.SetByOwner(key, Owner, oldValue + value);
        return state;
    }
}