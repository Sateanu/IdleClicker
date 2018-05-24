using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;

namespace IdleClicker
{
    class BuildingTile : Component
    {
        public bool Selected { get; set; }
        public bool Hovered { get; set; }
        public bool IsBuildable { get; set; }

        public TileManager Manager { get; set; }

        public Component Building { get; private set; }

        private Node m_Geometry;
        private Urho.Shapes.Plane m_Plane;
        private bool m_DestroyNextFrame;

        public BuildingTile[] Neighbors
        {
            get
            {
                return Manager.GetTileNeighbors(this);
            }
        }

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
            Hovered = false;
        }

        public void AddBuilding(BuildingProperties buildingProperties)
        {
            Debug.Assert(IsBuildable);

            Building = new Building(buildingProperties);
            Node.AddComponent(Building);
            Manager.AddBuilding();
        }

        public void AddDebris(DebrisProperties debrisProperties)
        {
            Debug.Assert(!IsBuildable);

            Building = new Debris(debrisProperties);
            Node.AddComponent(Building);
        }

        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);

            if(m_DestroyNextFrame)
            {
                DestroyBuilding();
            }

            if (Selected)
            {
                // TODO: investigate
                var mat = m_Plane.GetMaterial().Clone(); // seems shady, but simple SetShaderParameter doesn't work
                mat.SetShaderParameter("MatDiffColor", new Vector4(0.8f, 0.4f, 0.4f, 1f));
                m_Plane.SetMaterial(mat);
            }
            else if (Hovered)
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
            this.Node?.RemoveComponent(Building);

            m_Geometry = node.CreateChild();
            m_Plane = m_Geometry.CreateComponent<Urho.Shapes.Plane>();
            m_Plane.SetMaterial(Application.ResourceCache.GetMaterial(Assets.Materials.Grass));
        }

        internal bool HasBuildingBuilt()
        {
            return Building as Building != null;
        }

        internal void QueueDestroyBuilding()
        {
            m_DestroyNextFrame = true;
        }

        private void DestroyBuilding()
        {
            m_DestroyNextFrame = false;
            Building.Remove();
            Building = null;
            Manager.DeleteBuilding();
        }
    }
}
