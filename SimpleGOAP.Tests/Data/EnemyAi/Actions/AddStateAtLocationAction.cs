using System;
using System.Linq;
using SimpleGOAP;
using SimpleGOAP.KeyValueState;

namespace asgae.Ai.Actions;

public class AddStateAtLocation<T> : OwnerActionBase, IAction<OwnerKeyValueState> {
    private readonly string key;
    // FIXME: Need for dynamic can be removed with .net7+
    // https://learn.microsoft.com/en-us/dotnet/standard/generics/math
    // https://github.com/godotengine/godot/issues/71777
    private readonly dynamic value;
    private readonly dynamic minValue;
    private readonly dynamic maxValue;
    private readonly int[] location;

    public AddStateAtLocation(string owner, string key, T value, T minValue, T maxValue, int[] location = null) : base(owner)
    {
        this.key = key;
        this.value = value;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.location = location;
    }
    
    public string Title => $"AddStateAtLocationAction by {Owner}";

    public int GetCost(OwnerKeyValueState state)
    {
        return 1;
    }

    public bool PreconditionMet(OwnerKeyValueState state)
    {
        return (location == null || location.Contains(state.GetByOwner<int>(EnemyAiFactory.Location, Owner))) &&
               StateIsSafe(state);
    }

    public OwnerKeyValueState TakeActionOnState(OwnerKeyValueState state)
    {
        dynamic oldValue = state.GetByOwner<T>(key, Owner);
        state.SetByOwner(key, Owner, Math.Clamp(oldValue + value, minValue, maxValue));
        return state;
    }
}
