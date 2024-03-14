using System.Data.SqlTypes;
using asgae.Ai.Actions;
using SimpleGOAP.KeyValueState;

namespace asgae.Ai;

public class OwnerKeyValueState : KeyValueState<string, object>
{
        public static string OwnerStateKey(string key, string owner)
        {
            return $"state.{key}@{owner}";
        }
        
        public T GetByOwner<T>(string key, string owner)
        {
            return (T)Get<object>(OwnerStateKey(key, owner));
        }

        public void SetByOwner(string key, string owner, object value)
        {
            Set(OwnerStateKey(key, owner), value);
        }
}