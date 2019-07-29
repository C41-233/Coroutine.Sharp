using System;
using System.Collections.Generic;
using System.Linq;

namespace Coroutines.Signals
{

    public class SignalManager
    {

        private readonly Dictionary<Type, List<Action<ISignal>>> handlers = new Dictionary<Type, List<Action<ISignal>>>();

        private void OnSignal<T>(Action<T> action) where T : ISignal
        {
            var type = typeof(T);
            if (!handlers.TryGetValue(type, out var list))
            {
                list = new List<Action<ISignal>>();
                handlers.Add(type, list);
            }
            list.Add(signal => action((T)signal));
        }

        public sealed class Container
        {

            private readonly SignalManager manager;

            internal Container(SignalManager manager)
            {
                this.manager = manager;
            }

            public void OnSignal<T>(Action<T> action) where T : ISignal
            {

            }

        }


    }

}
