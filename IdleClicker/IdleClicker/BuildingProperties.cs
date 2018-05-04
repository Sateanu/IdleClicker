using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdleClicker
{
    public struct BuildingProperties
    {
        public IdlePlayerResourceType ResourceType { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public float Cost { get; set; }

        public float UpgradeCost { get; set; }
        public float UpgradeCostLevelMultiplier { get; set; }

        public float Reward { get; set; }
        public float RewardLevelMultiplier { get; set; }

        public float TimeForReward { get; set; }

        public string Model { get; set; }
        public string Material { get; set; }
    }
}
