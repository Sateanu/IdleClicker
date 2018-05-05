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
                Cost = 0.0f,
                CostLevelMultiplier = 1.0f,
                Reward = .5f,
                RewardLevelMultiplier = 1.0f,
                TimeToBuild = 3f,
                TimeForReward = .5f,
                ResourceType = IdlePlayerResourceType.Gold,
                Model = Assets.Models.House,
                Material = Assets.Materials.House,
                Scale = 0.05f,
            },

            new BuildingProperties()
            {
                Name = "Gold Mine",
                Cost = 10.0f,
                CostLevelMultiplier = 1.0f,
                Reward = 1.0f,
                RewardLevelMultiplier = 1.0f,
                TimeToBuild = 5f,
                TimeForReward = 1.0f,
                ResourceType = IdlePlayerResourceType.Gold,
                Model = Assets.Models.GoldMine,
                Material = Assets.Materials.GoldMine,
                Scale = 0.05f,
            },

            new BuildingProperties()
            {
                Name = "Bitcoin Farm",
                Cost = 58.0f,
                CostLevelMultiplier = 1.0f,
                Reward = 10.0f,
                RewardLevelMultiplier = 1.0f,
                TimeToBuild = 10f,
                TimeForReward = 1.0f,
                ResourceType = IdlePlayerResourceType.Gold,
                Model = Assets.Models.Tree,
                Material = Assets.Materials.Grass,
                Scale = 0.1f,
            },
        };

    }
}
