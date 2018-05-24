using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdleClicker
{
    public class DebrisData
    {
        Random m_Random;

        public DebrisData(int seed)
        {
            m_Random = new Random(seed);
        }

        public DebrisProperties GetNewDebris()
        {
            int typeId = (int)Math.Round((Debris.Count() - 1) * m_Random.NextDouble());
            DebrisPropertiesTemplate t = Debris[typeId];
            return new DebrisProperties
            {
                Model = t.Model,
                Material = t.Material,
                Scale = (float)(t.MinScale + m_Random.NextDouble() * (t.MaxScale - t.MinScale)),
                Rotation = (float)(Math.Round(m_Random.NextDouble() * 4) * 90),
                Type = (DebrisType)typeId
            };
        }

        private static DebrisPropertiesTemplate[] Debris =
        {
            new DebrisPropertiesTemplate()
            {
                Model = Assets.Models.Tree,
                Material = Assets.Materials.Tree,
                MinScale = 0.0002f,
                MaxScale = 0.0008f
            },

            new DebrisPropertiesTemplate()
            {
                Model = Assets.Models.Mountain,
                Material = Assets.Materials.House,
                MinScale = 0.08f,
                MaxScale = 0.15f
            },
        };
    }
}
