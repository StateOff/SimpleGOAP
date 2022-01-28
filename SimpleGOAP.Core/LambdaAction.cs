﻿using System;

namespace SimpleGOAP
{
    public class LambdaAction<T> : IAction<T>
    {
        private Func<T, bool> preconditionCheck;
        private Func<T, T> action;

        public LambdaAction(string title, int actionCost, Func<T, bool> preconditionCheck, Func<T, T> action)
        {
            Title = title;
            Cost = actionCost;
            this.preconditionCheck = preconditionCheck;
            this.action = action;
        }

        public LambdaAction(string title, int actionCost,  Func<T, T> action)
        {
            Title = title;
            Cost = actionCost;
            preconditionCheck = _ => true;
            this.action = action;
        }

        public LambdaAction(string title, int actionCost, Action<T> action)
        {
            Title = title;
            Cost = actionCost;
            preconditionCheck = _ => true;
            this.action = state =>
            {
                action(state);
                return state;
            };
        }

        public LambdaAction(string title, int actionCost, Func<T, bool> preconditionCheck, Action<T> action)
        {
            Title = title;
            Cost = actionCost;
            this.preconditionCheck = preconditionCheck;
            this.action = state =>
            {
                action(state);
                return state;
            };
        }


        public string Title { get; }
        public int Cost { get; }
        public bool IsLegalForState(T state) => preconditionCheck(state);
        public T TakeActionOnState(T state) => action(state);
    }
}
