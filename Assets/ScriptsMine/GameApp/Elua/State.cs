using System;

namespace Elua
{
    public struct State
    {
        public IntPtr L { get; }

        public State(IntPtr l)
        {
            L = l;
        }
        
    }
}