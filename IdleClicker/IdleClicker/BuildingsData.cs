using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdleClicker
{
    public class BuildingsData
    {
        public static BuildingProperties[] Buildings =
        {
            new BuildingProperties()
            {
                Name = "Farm",
                Cost = 1.0f,
                CostLevelMultiplier = 1.0f,
                Reward = 1f,
                RewardLevelMultiplier = 1.5f,
                TimeToBuild = 3f,
                TimeForReward = .5f,
                ResourceType = IdlePlayerResourceType.Gold,
                Model = Assets.Models.House,
                Material = Assets.Materials.House,
                Scale = 0.05f,
                BonusFactor = new Dictionary<DebrisType, float>
                {
                    {DebrisType.Tree, 2.0f},
                }
            },

            new BuildingProperties()
            {
                Name = "Gold Mine",
                Cost = 10.0f,
                CostLevelMultiplier = 1.0f,
                Reward = 4f,
                RewardLevelMultiplier = 2f,
                TimeToBuild = 5f,
                TimeForReward = 1.0f,
                ResourceType = IdlePlayerResourceType.Gold,
                Model = Assets.Models.GoldMine,
                Material = Assets.Materials.GoldMine,
                Scale = 0.05f,
                BonusFactor = new Dictionary<DebrisType, float>
                {
                    {DebrisType.Mountain, 1.5f},
                }
            },

            new BuildingProperties()
            {
                Name = "Bitcoin Farm",
                Cost = 58.0f,
                CostLevelMultiplier = 1.0f,
                Reward = 16f,
                RewardLevelMultiplier = 3f,
                TimeToBuild = 10f,
                TimeForReward = 1.0f,
                ResourceType = IdlePlayerResourceType.Gold,
                Model = Assets.Models.Pyramid,
                Material = Assets.Materials.Grass,
                Scale = 0.1f,
            },
        };

    }
}
