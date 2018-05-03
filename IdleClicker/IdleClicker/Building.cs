using System;
using System.Threading.Tasks;
using Urho;
using Urho.Actions;

namespace IdleClicker
{
    public class Building : Component
    {
        IdlePlayerResourceType ResourceType = IdlePlayerResourceType.Gold;
        int Cost = 10;
        int UpgradeCost = 10;

        float TimeForReward = 1.0f;
        int Reward = 1;

        private float TimeToReward;

        private Node m_Geometry;
        private Task<ActionState> m_ConstructionTask;

        public Building()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            TimeToReward = TimeForReward;
            ReceiveSceneUpdates = true;
        }

        protected override void OnUpdate(float timeStep)
        {
            if (m_ConstructionTask == null || !m_ConstructionTask.IsCompleted)
                return;

            TimeToReward -= timeStep;
            if (TimeToReward <= 0)
            {
                TimeToReward = TimeForReward;
                IdlePlayerManager.Instance.AddResourceValue(ResourceType, Reward);
            }
        }

        public override void OnAttachedToNode(Node node)
        {
            this.Node?.RemoveChild(m_Geometry);

            m_Geometry = node.CreateChild();
            var model = m_Geometry.CreateComponent<StaticModel>();
            model.Model = Application.ResourceCache.GetModel(Assets.Models.Tree);
            model.SetMaterial(Application.ResourceCache.GetMaterial(Assets.Materials.Grass));
            m_Geometry.SetScale(0.1f);
            m_Geometry.Position -= new Vector3(0f, 0.8f, 0f);

            m_ConstructionTask = m_Geometry.RunActionsAsync(new MoveTo(2f, new Vector3(0, 0, 0)));
        }
    }
}