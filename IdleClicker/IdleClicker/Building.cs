using System;
using Urho;

namespace IdleClicker
{
    public class Building
    {
        IdlePlayerResourceType ResourceType = IdlePlayerResourceType.Gold;
        int Cost = 10;
        int UpgradeCost = 10;

        float TimeForReward = 1.0f;
        int Reward = 1;

        private float TimeToReward;

        public Building()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            TimeToReward = TimeForReward;
        }

        internal void Update(float timeStep)
        {
            TimeToReward -= timeStep;
            if (TimeToReward <= 0)
            {
                TimeToReward = TimeForReward;
                IdlePlayerManager.Instance.AddResourceValue(ResourceType, Reward);
            }
        }
    }
}