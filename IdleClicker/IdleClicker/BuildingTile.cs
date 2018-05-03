using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;

namespace IdleClicker
{
    class BuildingTile : Component
    {
        public bool Selected { get; set; }

        private Building m_Building { get; set; }
        private Node m_Geometry;
        private Urho.Shapes.Plane m_Plane;

        public BuildingTile()
        {
            Initialize();
        }

        public BuildingTile(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public BuildingTile(Context context) : base(context)
        {
            Initialize();
        }

        protected BuildingTile(UrhoObjectFlag emptyFlag) : base(emptyFlag)
        {
            Initialize();
        }

        void Initialize()
        {
            ReceiveSceneUpdates = true;
            Selected = false;
        }

        public void AddBuilding(Building building)
        {
            m_Building = this.Node.CreateComponent<Building>();
        }

        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);

            if (Selected)
            {
                // TODO: investigate
                var mat = m_Plane.GetMaterial().Clone(); // seems shady, but simple SetShaderParameter doesn't work
                mat.SetShaderParameter("MatDiffColor", new Vector4(1f, 0.5f, 0.5f, 1f));
                m_Plane.SetMaterial(mat);
            }
            else
            {
                var mat = m_Plane.GetMaterial().Clone();
                mat.SetShaderParameter("MatDiffColor", new Vector4(1f, 1f, 1f, 1f));
                m_Plane.SetMaterial(mat);
            }


        }

        public override void OnAttachedToNode(Node node)
        {
            this.Node?.RemoveChild(m_Geometry);
            this.Node?.RemoveComponent(m_Building);

            m_Geometry = node.CreateChild();
            m_Plane = m_Geometry.CreateComponent<Urho.Shapes.Plane>();
            m_Plane.SetMaterial(Application.ResourceCache.GetMaterial(Assets.Materials.Grass));
        }
    }
}
