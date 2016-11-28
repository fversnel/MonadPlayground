using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonadPlayground {

    public sealed class Unit {
        public static readonly Unit Default = new Unit();

        private Unit() {}
    }
}
