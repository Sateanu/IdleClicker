using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Urho;
using Urho.Actions;
using Urho.Gui;
using Urho.Resources;

namespace IdleClicker
{
    public class Building : Component
    {
        public BuildingProperties BuildingProperties { get; private set; }

        private float TimeToReward;

        private Node m_Geometry;
        private Node m_TextNode;
        private Node m_LevelTextNode;

        private Text3D m_Text;
        private Text3D m_LevelText;

        private Task<ActionState> m_ConstructionTask;

        private int m_Level;
        public int Level
        {
            get { return m_Level; }
            set
            {
                m_Level = value;
                m_LevelText.Text = m_Level.ToString();
            }
        }

        public IEnumerable<DebrisType> Neighbors
        {
            get
            {
                return Node.GetComponent<BuildingTile>().Neighbors
                                                        .Select(n => n.Node.GetComponent<Debris>())
                                                        .Where(n => n != null)
                                                        .Select(n => n.DebrisType);
            }
        }

        public Building(BuildingProperties buildingProperties)
        {
            BuildingProperties = buildingProperties;
            ReceiveSceneUpdates = true;
        }

        public virtual void Initialize()
        {
            TimeToReward = BuildingProperties.TimeForReward;
            Level = 1;
        }

        float m_TextAnimationFactor = 1.5f;

        protected override void OnUpdate(float timeStep)
        {
            if (m_Geometry == null || m_ConstructionTask == null || !m_ConstructionTask.IsCompleted)
                return;

            TimeToReward -= timeStep;

            // Hack until proper FadeOut implementation
            m_Text.Opacity = TimeToReward * 2.2f / BuildingProperties.TimeForReward;

            if (TimeToReward <= 0)
            {
                TimeToReward = BuildingProperties.TimeForReward;

                float reward = BuildingProperties.GetRewardForLevel(Level, Neighbors);
                IdlePlayerManager.Instance.AddResourceValue(BuildingProperties.ResourceType, reward);

                m_Text.Text = ((int)Math.Round(reward)).ToString();

                m_TextNode.Position = new Vector3(0f, 0f, 0f);
                m_TextNode.RunActionsAsync(new MoveTo(BuildingProperties.TimeForReward / m_TextAnimationFactor, new Vector3(0, 1.2f, 0)));
            }
        }

        public override void OnAttachedToNode(Node node)
        {
            this.Node?.RemoveChild(m_Geometry);
            this.Node?.RemoveChild(m_TextNode);
            this.Node?.RemoveChild(m_LevelTextNode);

            m_TextNode = node.CreateChild();
            //m_TextNode.LookAt(new Vector3(0,-10,10), Vector3.Up, TransformSpace.World); // camera pos hardcoded !!! TODO: fix
            m_TextNode.Position += new Vector3(0f, 1f, 0f);
            m_Text = m_TextNode.CreateComponent<Text3D>();
            m_Text.SetFont(CoreAssets.Fonts.AnonymousPro, 20);
            m_Text.SetColor(Color.Yellow);
            m_Text.EffectColor = Color.Black;
            m_Text.TextEffect = TextEffect.Shadow;

            m_LevelTextNode = node.CreateChild();
            m_LevelTextNode.LookAt(new Vector3(-1f, -1f, 1f), Vector3.Up, TransformSpace.Local); // camera pos hardcoded !!! TODO: fix
            m_LevelTextNode.Position += new Vector3(0.25f, 0.5f, -0.5f);
            m_LevelText = m_LevelTextNode.CreateComponent<Text3D>();
            m_LevelText.SetFont(CoreAssets.Fonts.AnonymousPro, 30);
            m_LevelText.SetColor(Color.Green);
            m_LevelText.EffectColor = Color.Black;
            m_LevelText.TextEffect = TextEffect.Shadow;
            m_LevelText.Text = Level.ToString();

            m_Geometry = node.CreateChild();
            var model = m_Geometry.CreateComponent<StaticModel>();
            model.Model = Application.ResourceCache.GetModel(BuildingProperties.Model);
            model.SetMaterial(Application.ResourceCache.GetMaterial(BuildingProperties.Material));
            m_Geometry.SetScale(BuildingProperties.Scale);
            m_Geometry.Position -= new Vector3(0f, 0.8f, 0f);

            m_ConstructionTask = m_Geometry.RunActionsAsync(new MoveTo(BuildingProperties.TimeToBuild, new Vector3(0, 0, 0)));

            Initialize();
        }

        protected override void OnDeleted()
        {
            base.OnDeleted();
            ReceiveSceneUpdates = false;
            if (!m_Geometry.IsDeleted)
            {
                m_Geometry.Remove();
                m_Geometry = null;
                m_Text.Remove();
                m_TextNode.Remove();
                m_LevelText.Remove();
                m_LevelTextNode.Remove();
            }
        }
    }
}