using SimpleGOAP;
using SimpleGOAP.KeyValueState;

namespace asgae.Ai.Actions;

public class OwnerActionBase {
    
    public string Owner;

    public OwnerActionBase(string owner)
    {
        Owner = owner;
    }
}