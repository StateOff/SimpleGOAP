using SimpleGOAP;
using SimpleGOAP.KeyValueState;

namespace asgae.Ai.Actions;

public class OwnerActionBase {
    
    public string Owner;

    public OwnerActionBase(string owner)
    {
        Owner = owner;
    }

    protected bool StateIsSafe(OwnerKeyValueState state)
    {
        return state.GetByOwner<bool>(EnemyAiFactory.KeyIsThreatened, Owner) == false &&
               state.GetByOwner<float>(EnemyAiFactory.KeyAttention, Owner) <= EnemyAiFactory.AttentionState.AtEaseF;
    }
    
    protected bool StateIsOnLocation(OwnerKeyValueState state, int location = 0)
    {
        return state.GetByOwner<int>(EnemyAiFactory.Location, Owner) == location;
    }
}