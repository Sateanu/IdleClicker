using System;
using System.Threading.Tasks;
using Urho;
using Urho.Actions;
using Urho.Gui;

namespace IdleClicker
{
    public class Debris : Component
    {
        private DebrisProperties DebrisProperties;

        private Node m_Geometry;

        public DebrisType DebrisType
        {
            get { return DebrisProperties.Type; }
        }

        public Debris(DebrisProperties debrisProperties)
        {
            Initialize(debrisProperties);
        }

        public virtual void Initialize(DebrisProperties debrisProperties)
        {
            DebrisProperties = debrisProperties;
        }

        public override void OnAttachedToNode(Node node)
        {
            this.Node?.RemoveChild(m_Geometry);

            m_Geometry = node.CreateChild();
            var model = m_Geometry.CreateComponent<StaticModel>();
            model.Model = Application.ResourceCache.GetModel(DebrisProperties.Model);
            model.SetMaterial(Application.ResourceCache.GetMaterial(DebrisProperties.Material));
            m_Geometry.SetScale(DebrisProperties.Scale);
            m_Geometry.Yaw(DebrisProperties.Rotation);
        }
    }
}