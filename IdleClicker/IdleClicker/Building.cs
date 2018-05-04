using System;
using System.Threading.Tasks;
using Urho;
using Urho.Actions;

namespace IdleClicker
{
    public class Building : Component
    {
        private BuildingProperties BuildingProperties;

        private float TimeToReward;

        private Node m_Geometry;
        private Task<ActionState> m_ConstructionTask;

        public int Level { get; set; }

        public Building(BuildingProperties buildingProperties)
        {
            Initialize(buildingProperties);
        }

        public virtual void Initialize(BuildingProperties buildingProperties)
        {
            BuildingProperties = buildingProperties;
            TimeToReward = BuildingProperties.TimeForReward;
            ReceiveSceneUpdates = true;
        }

        protected override void OnUpdate(float timeStep)
        {
            if (m_ConstructionTask == null || !m_ConstructionTask.IsCompleted)
                return;

            TimeToReward -= timeStep;
            if (TimeToReward <= 0)
            {
                TimeToReward = BuildingProperties.TimeForReward;
                IdlePlayerManager.Instance.AddResourceValue(BuildingProperties.ResourceType, BuildingProperties.Reward);
            }
        }

        public override void OnAttachedToNode(Node node)
        {
            this.Node?.RemoveChild(m_Geometry);

            m_Geometry = node.CreateChild();
            var model = m_Geometry.CreateComponent<StaticModel>();
            model.Model = Application.ResourceCache.GetModel(BuildingProperties.Model);
            model.SetMaterial(Application.ResourceCache.GetMaterial(BuildingProperties.Material));
            m_Geometry.SetScale(0.1f);
            m_Geometry.Position -= new Vector3(0f, 0.8f, 0f);

            m_ConstructionTask = m_Geometry.RunActionsAsync(new MoveTo(2f, new Vector3(0, 0, 0)));
        }
    }
}