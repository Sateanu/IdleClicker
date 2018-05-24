using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdleClicker
{
    public class BuildingProperties
    {
        public IdlePlayerResourceType ResourceType { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public float Cost { get; set; }
        public float CostLevelMultiplier { get; set; }

        public float Reward { get; set; }
        public float RewardLevelMultiplier { get; set; }

        public float TimeToBuild { get; set; }
        public float TimeForReward { get; set; }

        public string Model { get; set; }
        public string Material { get; set; }

        public float Scale { get; set; }

        public Dictionary<DebrisType, float> BonusFactor { get; set; }

        public float GetRewardForLevel(int Level, IEnumerable<DebrisType> Neighbors)
        {
            float factor = BonusFactor == null ? 1.0f :
                Neighbors.Select(
                n =>
                {
                    float f = 1.0f;
                    if (BonusFactor.TryGetValue(n, out f))
                        return f;

                    return 1f;
                }).Aggregate(1f, (acc, val) => acc * val);

            return Reward * (float)Math.Pow(RewardLevelMultiplier, (Level - 1)) * factor;
        }

        public float GetCostForLevel(int Level)
        {
            return (float)Math.Pow(Cost + ((Level - 1) * CostLevelMultiplier), ((Level - 1) * CostLevelMultiplier));
        }
    }
}
