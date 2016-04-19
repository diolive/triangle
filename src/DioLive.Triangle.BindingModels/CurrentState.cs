using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DioLive.Triangle.BindingModels
{
    public class CurrentState
    {
        public bool Alive { get; }

        public bool Stunned { get; }

        public float? Beaming { get; }
    }
}
