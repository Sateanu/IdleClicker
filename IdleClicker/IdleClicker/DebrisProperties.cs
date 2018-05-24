using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdleClicker
{
    public enum DebrisType
    {
        Tree,
        Mountain
    }

    public struct DebrisProperties
    {
        public string Model { get; set; }
        public string Material { get; set; }

        public float Scale { get; set; }
        public float Rotation { get; set; }

        public DebrisType Type { get; set; }
    }

    public struct DebrisPropertiesTemplate
    {
        public string Model { get; set; }
        public string Material { get; set; }

        public float MinScale { get; set; }
        public float MaxScale { get; set; }
    }
}
