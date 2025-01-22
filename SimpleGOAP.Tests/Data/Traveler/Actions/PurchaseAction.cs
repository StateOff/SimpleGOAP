﻿using System;
using SimpleGOAP.KeyValueState;

namespace SimpleGOAP.Tests.Data.Traveler.Actions
{
    public class PurchaseAction : IAction<KeyValueState<string, object>>
    {
        private readonly string itemName;
        private readonly string storeName;
        private readonly int cost;
        private readonly int amountPerPurchase;
        private readonly int? max;

        public PurchaseAction(string itemName, string storeName, int cost, int amountPerPurchase, int? max = null)
        {
            this.max = max;
            this.itemName = itemName;
            this.storeName = storeName;
            this.cost = cost;
            this.amountPerPurchase = amountPerPurchase;
        }

        public string Title => $"Purchase {itemName} x{amountPerPurchase} for ${cost}";
        public int GetCost(KeyValueState<string, object> state) => 10;

        public bool PreconditionMet(KeyValueState<string, object> state)
        {
            return state.Get<int>("money") >= cost
                   && state.Check("myLocation", storeName)
                   && (max == null || state.Get<int>(itemName) < max);
        }

        public KeyValueState<string, object> TakeActionOnState(KeyValueState<string, object> state, bool isPlanning)
        {
            state.Set(itemName, Math.Min(max ?? int.MaxValue, state.Get<int>(itemName) + amountPerPurchase));
            state.Set("money", state.Get<int>("money") - cost);
            return state;
        }
    }
}
