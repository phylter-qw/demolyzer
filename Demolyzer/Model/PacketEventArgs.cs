using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model
{
    public class PackEventArgs : EventArgs
    {
        public Entity Pack { get; private set; }

        public PackEventArgs(Entity pack)
        {
            this.Pack = pack;
        }
    }
}
