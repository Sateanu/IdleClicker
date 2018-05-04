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
                Cost = 5.0f,
                Reward = .5f,
                RewardLevelMultiplier = 1.0f,
                TimeForReward = 1.0f,
                ResourceType = IdlePlayerResourceType.Gold,
                Model = Assets.Models.Player,
                Material = Assets.Materials.Player,
                UpgradeCost = 10.0f,
                UpgradeCostLevelMultiplier = 1.0f
            },

            new BuildingProperties()
            {
                Name = "Gold Mine",
                Cost = 10.0f,
                Reward = 1.0f,
                RewardLevelMultiplier = 1.0f,
                TimeForReward = 1.0f,
                ResourceType = IdlePlayerResourceType.Gold,
                Model = Assets.Models.Tree,
                Material = Assets.Materials.Pyramid,
                UpgradeCost = 10.0f,
                UpgradeCostLevelMultiplier = 1.0f
            },

            new BuildingProperties()
            {
                Name = "Bitcoin Farm",
                Cost = 100.0f,
                Reward = 10.0f,
                RewardLevelMultiplier = 1.0f,
                TimeForReward = 1.0f,
                ResourceType = IdlePlayerResourceType.Gold,
                Model = Assets.Models.Tree,
                Material = Assets.Materials.Grass,
                UpgradeCost = 10.0f,
                UpgradeCostLevelMultiplier = 1.0f
            },
        };

    }
}
