using Microsoft.Extensions.Logging;

namespace SimpleGOAP.KeyValueState
{
    public class KeyValuePlanner : Planner<KeyValueState<string, object>>
    {
        public KeyValuePlanner(ILogger<KeyValuePlanner> logger=null) : base(new KeyValueStateCopier<string, object>(), new KeyValueStateComparer<string, object>(), logger)
        {
        }
    }
}
