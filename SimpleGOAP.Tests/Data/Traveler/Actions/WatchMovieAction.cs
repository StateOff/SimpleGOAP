﻿using SimpleGOAP.KeyValueState;

namespace SimpleGOAP.Tests.Data.Traveler.Actions
{
    public class WatchMovieAction : IAction<KeyValueState<string, object>>
    {
        public string Title => "Watch movie for $20";
        public int GetCost(KeyValueState<string, object> state) => 10;

        public bool PreconditionMet(KeyValueState<string, object> state)
        {
            return state.Check("myLocation", "Theater") && state.Get<int>("money") >= 20;
        }

        public KeyValueState<string, object> TakeActionOnState(KeyValueState<string, object> state, bool isPlanning)
        {
            state.Set("money", state.Get<int>("money") - 20);
            state.Set("fun", state.Get<int>("fun") + 1);
            return state;
        }
    }
}
