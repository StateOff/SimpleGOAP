using System;

namespace SimpleGOAP
{
    public class LambdaAction<T> : IAction<T>
    {
        private Func<T, T> action;
        private Func<T, bool> precondition;
        private readonly Func<int> getCost;

        public LambdaAction(string title, int actionCost, Func<T, T> action, Func<T, bool> precondition=null)
        {
            Title = title;
            getCost = () => actionCost;
            this.action = action;
            this.precondition = precondition;
        }

        public LambdaAction(string title, int actionCost, Action<T> action, Func<T, bool> precondition=null)
        {
            Title = title;
            getCost = () => actionCost;
            this.action = state =>
            {
                action(state);
                return state;
            };
            this.precondition = precondition;
        }

        public LambdaAction(string title, Func<int> getCost, Func<T, T> action, Func<T, bool> precondition=null)
        {
            Title = title;
            this.getCost = getCost;
            this.action = action;
            this.precondition = precondition;
        }

        public LambdaAction(string title, Func<int> getCost, Action<T> action, Func<T, bool> precondition=null)
        {
            Title = title;
            this.getCost = getCost;
            this.action = state =>
            {
                action(state);
                return state;
            };
            this.precondition = precondition;
        }

        public LambdaAction(string title, Func<T, T> action, Func<T, bool> precondition=null)
        {
            Title = title;
            getCost = () => 1;
            this.action = action;
            this.precondition = precondition;
        }

        public LambdaAction(string title, Action<T> action, Func<T, bool> precondition=null)
        {
            Title = title;
            getCost = () => 1;
            this.action = state =>
            {
                action(state);
                return state;
            };
            this.precondition = precondition;
        }

        public override string ToString()
        {
            return Title;
        }

        public string Title { get; }

        public int GetCost(T state) => getCost();
        
        public bool PreconditionMet(T state)
        {
            return precondition == null || precondition(state);
        }

        public T TakeActionOnState(T state) => action(state);
    }
}
