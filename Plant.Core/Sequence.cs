using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plant.Core
{
    public interface ISequence
    {
        Delegate Func { get; }
    }
    public class Sequence<TResult> : ISequence
    {
        public Sequence(Func<int, TResult> func)
        {
            Func = func;
        }

        public Delegate Func { get; private set; }
    }
}
