using System;
using System.Collections.Generic;
using System.Linq;
using asgae.Ai.Actions;
using SimpleGOAP;
using SimpleGOAP.KeyValueState;

namespace asgae.Ai
{
    public static class EnemyAiFactory
    {
        public class OwnerKeyValueStateCopier : IStateCopier<OwnerKeyValueState> 
        {
            public OwnerKeyValueState Copy(OwnerKeyValueState state)
            {
                var newState = new OwnerKeyValueState();
                foreach (var fact in state.Facts)
                    newState.Set(fact);

                return newState;
            }
        }
        
        public class OwnerKeyValuePlanner : Planner<OwnerKeyValueState>
        {
            public OwnerKeyValuePlanner() : base(new OwnerKeyValueStateCopier(), new KeyValueStateComparer<string, object>())
            {
            }
        }
        
        public static class AttentionState
        {
            public enum AttentStateEnum
            {
                Neutralized = 0,
                Distracted = 5,
                Occupied = 10,
                AtEase = 25,
                Attentive = 40,
                Alert = 60,
                InDanger = 80,
            }

            private const float _PerCent = 100.0f;
            
            // -- State Values normalized between 0.0f and 1.0f
            // --
            public const float NeutralizedF = (int)AttentStateEnum.Neutralized / _PerCent;
            public const float DistractedF  = (int)AttentStateEnum.Distracted  / _PerCent;
            public const float OccupiedF    = (int)AttentStateEnum.Occupied    / _PerCent;
            public const float AtEaseF      = (int)AttentStateEnum.AtEase      / _PerCent;
            public const float AttentiveF   = (int)AttentStateEnum.Attentive   / _PerCent;
            public const float AlertF       = (int)AttentStateEnum.Alert       / _PerCent;
            public const float InDangerF    = (int)AttentStateEnum.InDanger    / _PerCent;

            /// <summary>
            /// Return a State, given a normalized attention value between 0.0f and 1.0f
            /// </summary>
            /// <param name="value">Normalized attention value</param>
            /// <returns>State</returns>
            public static AttentStateEnum ToState(float value)
            {
                int valueNumber = (int)(value * _PerCent);
                AttentStateEnum lastAttentStateEnum = AttentStateEnum.Neutralized;
                foreach (int currentState in Enum.GetValues<AttentStateEnum>())
                {
                    if (valueNumber > currentState)
                    {
                        break;
                    }
                    lastAttentStateEnum = (AttentStateEnum)currentState;
                }

                return lastAttentStateEnum;
            }
        }
        
        public const string KeyHealth = "health";
        public const string KeyFatigue = "fatigue";
        public const string KeyBoredom = "boredom";
        public const string KeyHunger = "hunger";
        public const string KeyAttention = "attention";
        public const string KeyIsThreatened = "is_threatened";
        public const string KeyMoney = "money";
        public const string KeyIsSmoker = "is_smoker";
        public const string KeyCigarettes = "cigarettes";
        public const string KeyFood = "food";
        public const string Location = "location";
        
        public static (PlanParameters<OwnerKeyValueState>, OwnerKeyValuePlanner) Create(string name)
        {
            const float BOREDOM_RATE_PER_HOUR = 0.1f;
            const float BOREDOM_RATE_PER_HOUR_THREATENED = 0.0f;
            const float ATTENTION_LOSS_PER_HOUR = 0.1f;
            const float ATTENTION_LOSS_PER_HOUR_THREATENED = 0.1f;
        

            string healthOwnerKey = OwnerKeyValueState.OwnerStateKey(KeyHealth, name);
            string isThreatenedOwnerKey = OwnerKeyValueState.OwnerStateKey(KeyIsThreatened, name);
            
            string fatigueOwnerKey = OwnerKeyValueState.OwnerStateKey(KeyFatigue, name);
            string boredomOwnerKey = OwnerKeyValueState.OwnerStateKey(KeyBoredom, name);
            string hungerOwnerKey = OwnerKeyValueState.OwnerStateKey(KeyHunger, name);
            string attentionOwnerKey = OwnerKeyValueState.OwnerStateKey(KeyAttention, name);
            string moneyOwnerKey = OwnerKeyValueState.OwnerStateKey(KeyMoney, name);
            string isSmokerOwnerKey = OwnerKeyValueState.OwnerStateKey(KeyIsSmoker, name);
            string cigarettesOwnerKey = OwnerKeyValueState.OwnerStateKey(KeyCigarettes, name);
            string foodOwnerKey = OwnerKeyValueState.OwnerStateKey(KeyFood, name);
            string locationOwnerKey = OwnerKeyValueState.OwnerStateKey(Location, name);
            
            string cigarettesCarKey = OwnerKeyValueState.OwnerStateKey(KeyCigarettes, "car");

            var currentState = new OwnerKeyValueState()
            {
                {healthOwnerKey,  100},
                {fatigueOwnerKey,  0.25f},
                {boredomOwnerKey,  0.3f},
                {hungerOwnerKey,  0.3f},
                {attentionOwnerKey,  AttentionState.AtEaseF},
                {isThreatenedOwnerKey,  false},
                {moneyOwnerKey,  20},
                {isSmokerOwnerKey,  true},
                {cigarettesOwnerKey, 7},
                {cigarettesCarKey, 8},
                {foodOwnerKey,  1},
                {locationOwnerKey,  0},
            };

            IEnumerable<IAction<OwnerKeyValueState>> GetActions(OwnerKeyValueState state)
            {
                {
                    var eatFoodAction = new EatFoodAction(name);
                    if (eatFoodAction.PreconditionMet(state))
                        yield return eatFoodAction;
                }
                
                {
                    var smokeAction = new SmokeAction(name);
                    if (smokeAction.PreconditionMet(state))
                        yield return smokeAction;
                }

                {
                    var walkAroundAction = new WalkAroundAction(name);
                    if (walkAroundAction.PreconditionMet(state))
                        yield return walkAroundAction;
                }
                
                {
                    var gotoCarAction = new GotoAction(name, new []{0}, 1);
                    if (gotoCarAction.PreconditionMet(state))
                        yield return gotoCarAction;
                }
                
                {
                    var goOutOfCarAction = new GotoAction(name, new []{1}, 0);
                    if (goOutOfCarAction.PreconditionMet(state))
                        yield return goOutOfCarAction;
                }
                
                {
                    var getCigarettesFromCar = new SwapStateAtLocationAction<int>("car", name, KeyCigarettes, 8, true, new []{1});
                    if (getCigarettesFromCar.PreconditionMet(state))
                        yield return getCigarettesFromCar;
                }
                
                {
                    var driveAwayAction = new DriveAwayAction(name);
                    if (driveAwayAction.PreconditionMet(state))
                        yield return driveAwayAction;
                }
                
                {
                    var driveAwayAction = new DriveAwayAction(name);
                    if (driveAwayAction.PreconditionMet(state))
                        yield return driveAwayAction;
                }

                {
                    var idleAction = new IdleAction(name);
                    if (idleAction.PreconditionMet(state))
                        yield return idleAction;
                }
            }

            int HeuristicCost(OwnerKeyValueState state) =>
                new[]
                {
                    50 * (3 - Math.Min(3, state.Get<int>(healthOwnerKey) / 20)),
                    state.Check(isThreatenedOwnerKey, false) ? 0 : 50,
                    
                    10 * (int)MathF.Ceiling(state.Get<float>(hungerOwnerKey) * 1.0f - 0.0f),
                    (int)MathF.Ceiling(state.Get<float>(boredomOwnerKey) * 1.0f - 0.2f),
                    
                }.Sum();
            
            bool OkGoal(OwnerKeyValueState state)
            {
                return
                    state.Get<float>(fatigueOwnerKey) <= 0.31f &&
                    state.Get<float>(boredomOwnerKey) <= 0.21f &&
                    state.Get<float>(hungerOwnerKey) <= 0.21f;
            }

            var args = new PlanParameters<OwnerKeyValueState>
            {
                GetActions = GetActions,
                StartingState = currentState,
                HeuristicCost = HeuristicCost,
                MaxIterations = 1000,
                GoalsEvaluator = new() { OkGoal },
            };

            return (args, new OwnerKeyValuePlanner());
        }
    }
}
