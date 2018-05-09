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
            model.Model = Application.ResourceCache.GetModel(Assets.Models.Player);
            model.SetMaterial(Application.ResourceCache.GetMaterial(Assets.Materials.Player));
            m_Geometry.SetScale(0.1f);
        }
    }
}