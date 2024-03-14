using System.Collections.Generic;
using System.Linq;
using SimpleGOAP.KeyValueState;
using SimpleGOAP.Tests.Data.Traveler.Actions;

namespace SimpleGOAP.Tests.Data.Traveler
{
    public static class TravelerDataFactory
    {
        public static (PlanParameters<KeyValueState<string, object>>, Planner<KeyValueState<string, object>>) Create()
        {
            const int COST_OF_TOY = 10;
            const int SELL_VALUE_OF_TOY = 30;
            const int COST_OF_GAS = 50;
            const int COST_OF_FOOD = 30;
            const int GAS_TANK_CAPACITY = 40;
            const int WAGE = 20;

            var currentState = new KeyValueState<string, object>();
            foreach (var (key, val) in new (string, object)[]
                     {
                         ("myLocation", "Home"),
                         ("food", 0),
                         ("full", false),
                         ("money", 0),
                         ("gas", 40),
                         ("fun", 0),
                         ("fatigue", 0),
                         ("toy", 0),
                     })
            {
                currentState.Set(new Fact<string, object>(key, val));
            }


            var locations = new List<(string, int, int)>
            {
                ("Restaurant", 2, 2),
                ("Work", 1, 0),
                ("Gas Station", 1, 1),
                ("Home", 0, 0),
                ("Theater", 2, 0),
            };
            
            IEnumerable<IAction<KeyValueState<string, object>>> GetActions(KeyValueState<string, object> state)
            {
                var restaurantDriveAction = new DriveAction("Restaurant", locations);
                if(restaurantDriveAction.PreconditionMet(state))
                    yield return restaurantDriveAction;

                var workDriveAction = new DriveAction("Work", locations);
                if(workDriveAction.PreconditionMet(state))
                    yield return workDriveAction;

                var homeDriveAction = new DriveAction("Home", locations);
                if(homeDriveAction.PreconditionMet(state))
                    yield return homeDriveAction;
                
                var gasStationDriveAction =  new DriveAction("Gas Station", locations);
                if(gasStationDriveAction.PreconditionMet(state))
                    yield return gasStationDriveAction;
                
                var theaterDriveAction = new DriveAction("Theater", locations);
                if(theaterDriveAction.PreconditionMet(state))
                    yield return theaterDriveAction;
                
                var purchaseGasAction = new PurchaseAction("gas", "Gas Station", COST_OF_GAS, GAS_TANK_CAPACITY, GAS_TANK_CAPACITY);
                if(purchaseGasAction.PreconditionMet(state))
                    yield return purchaseGasAction;
                
                var purchaseToyAction = new PurchaseAction("toy", "Gas Station", COST_OF_TOY, 3, 6);
                if(purchaseToyAction.PreconditionMet(state))
                    yield return purchaseToyAction;

                var purchaseFoodAction = new PurchaseAction("food", "Restaurant", COST_OF_FOOD, 1);
                if(purchaseFoodAction.PreconditionMet(state))
                    yield return purchaseFoodAction;

                var sellToyAction = new SellAction("toy", SELL_VALUE_OF_TOY);
                if(sellToyAction.PreconditionMet(state))
                    yield return sellToyAction;

                var workAction = new WorkAction("Work", WAGE);
                if(workAction.PreconditionMet(state))
                    yield return workAction;
                
                var watchMovieAction = new WatchMovieAction();
                if(watchMovieAction.PreconditionMet(state))
                    yield return watchMovieAction;
                
                var sleepAction = new SleepAction();
                if(sleepAction.PreconditionMet(state))
                    yield return sleepAction;
                
                var eatAction = new EatAction();
                if(eatAction.PreconditionMet(state))
                    yield return eatAction;
            }

            int HeuristicCost(KeyValueState<string, object> state) =>
                new[]
                {
                    state.Check("full", true) ? 0 : 1,
                    state.Check("myLocation", "Home") ? 0 : 1,
                    state.Get<int>("fun") >= 2 ? 0 : 1,
                    state.Get<int>("fatigue") <= 0 ? 0 : 1,
                }.Sum() * 300;

            var args = new PlanParameters<KeyValueState<string, object>>
            {
                GetActions = GetActions,
                StartingState = currentState,
                HeuristicCost = HeuristicCost,
                GoalEvaluator = s => HeuristicCost(s) <= 0,
            };

            return (args, new KeyValuePlanner());
        }
    }
}
