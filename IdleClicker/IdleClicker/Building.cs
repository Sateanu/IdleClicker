using System;
using System.Threading.Tasks;
using Urho;
using Urho.Actions;
using Urho.Gui;

namespace IdleClicker
{
    public class Building : Component
    {
        public BuildingProperties BuildingProperties { get; private set; }

        private float TimeToReward;

        private Node m_Geometry;
        private Node m_TextNode;

        private Text3D m_Text;

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
            Level = 1;
        }

        protected override void OnUpdate(float timeStep)
        {
            if (m_ConstructionTask == null || !m_ConstructionTask.IsCompleted)
                return;

            TimeToReward -= timeStep;

            // Hack until proper FadeOut implementation
            m_Text.Opacity = TimeToReward * 2 / BuildingProperties.TimeForReward;

            if (TimeToReward <= 0)
            {
                TimeToReward = BuildingProperties.TimeForReward;
                float reward = BuildingProperties.GetRewardForLevel(Level);
                IdlePlayerManager.Instance.AddResourceValue(BuildingProperties.ResourceType, reward);

                m_Text.Text = reward.ToString();

                m_TextNode.Position = new Vector3(0f, 0f, 0f);
                m_TextNode.RunActionsAsync(new MoveTo(BuildingProperties.TimeForReward / 2, new Vector3(0, 1.2f, 0)));
            }
        }

        public override void OnAttachedToNode(Node node)
        {
            this.Node?.RemoveChild(m_Geometry);
            this.Node?.RemoveChild(m_TextNode);

            m_TextNode = node.CreateChild();
            //m_TextNode.LookAt(new Vector3(0,-10,10), Vector3.Up, TransformSpace.World); // camera pos hardcoded !!! TODO: fix
            m_TextNode.Position += new Vector3(0f, 1f, 0f);
            m_Text = m_TextNode.CreateComponent<Text3D>();
            m_Text.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            m_Text.SetColor(Color.Yellow);
            m_Text.EffectColor = Color.Black;
            m_Text.TextEffect = TextEffect.Shadow;

            m_Geometry = node.CreateChild();
            var model = m_Geometry.CreateComponent<StaticModel>();
            model.Model = Application.ResourceCache.GetModel(BuildingProperties.Model);
            model.SetMaterial(Application.ResourceCache.GetMaterial(BuildingProperties.Material));
            m_Geometry.SetScale(BuildingProperties.Scale);
            m_Geometry.Position -= new Vector3(0f, 0.8f, 0f);

            m_ConstructionTask = m_Geometry.RunActionsAsync(new MoveTo(BuildingProperties.TimeToBuild, new Vector3(0, 0, 0)));
        }
    }
}