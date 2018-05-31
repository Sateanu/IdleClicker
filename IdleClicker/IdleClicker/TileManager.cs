using System;
using System.Collections.Generic;
using System.Diagnostics;
using Urho;

namespace IdleClicker
{
    class TileManager : Component
    {
        const int INITIAL_SIZE = 9;
        int m_CurrentSize = INITIAL_SIZE;

        int m_BuildableTiles = 0;
        int m_OccupiedTiles = 0;

        const float BUILDABLE_RATIO = 0.8f;
        const float EXTEND_RATIO = 0.2f;

        private int m_Seed;
        private Random m_RndGen;
        private DebrisData m_DebrisData;

        public TileManager()
        {
            InitializeDefault();
        }

        public TileManager(IntPtr handle)
            : base(handle)
        {
            InitializeDefault();
        }

        public TileManager(Context context)
            : base(context)
        {
            InitializeDefault();
        }

        public TileManager(int seed)
        {
            Initialize(seed);
        }

        private void InitializeDefault()
        {
            Initialize((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }

        private void Initialize(int seed)
        {
            m_Seed = seed;
            m_RndGen = new Random(m_Seed);
            m_DebrisData = new DebrisData(m_Seed);
        }

        private static string GetPosString(int x, int z)
        {
            return "" + x + "_" + z;
        }

        private static IntVector2 GetStringPos(string pos)
        {
            var xz = pos.Split('_');
            IntVector2 ret = new IntVector2();
            ret.X = Int32.Parse(xz[0]);
            ret.Y = Int32.Parse(xz[1]);

            return ret;
        }

        public BuildingTile GetTile(int x, int z)
        {
            var node = Node.GetChild(GetPosString(x, z));
            return node?.GetComponent<BuildingTile>();
        }

        public BuildingTile[] GetTileNeighbors(BuildingTile target)
        {
            var pos = GetStringPos(target.Node.Name);
            return GetTileNeighbors(pos.X, pos.Y);
        }

        public BuildingTile[] GetTileNeighbors(int x, int z)
        {
            List<BuildingTile> ret = new List<BuildingTile>();

            int[] xs = new int[] { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] zs = new int[] { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int i = 0; i < xs.Length; i++)
            {
                BuildingTile bt = GetTile(x + xs[i], z + zs[i]);
                if (bt != null)
                    ret.Add(bt);
            }

            return ret.ToArray();
        }

        private void AddTile(Node node, int x, int z)
        {
            var childTile = node.CreateChild(GetPosString(x, z));
            childTile.AddTag("tile");
            childTile.Position = new Vector3(x, 0, z);

            var tileComp = childTile.CreateComponent<BuildingTile>();
            bool isBuildable = m_RndGen.NextDouble() < BUILDABLE_RATIO;

            tileComp.IsBuildable = isBuildable;
            tileComp.Manager = this;

            if (isBuildable)
                m_BuildableTiles++;
            else
                tileComp.AddDebris(m_DebrisData.GetNewDebris());
        }

        public override void OnAttachedToNode(Node node)
        {
            int offset = m_CurrentSize / 2;

            for (int x = -offset; x < offset; x++)
            {
                for (int z = -offset; z < offset; z++)
                {
                    AddTile(node, x, z);
                }
            }
        }

        public void AddBuilding()
        {
            m_OccupiedTiles++;

            if ((float)m_OccupiedTiles / m_BuildableTiles > EXTEND_RATIO)
                Extend();
        }

        public void DeleteBuilding()
        {
            m_OccupiedTiles--;
        }

        private void Extend()
        {
            int lastOffset = m_CurrentSize / 2;
            int offset = m_CurrentSize;
            m_CurrentSize *= 2;

            for (int x = -offset; x < -lastOffset; x++)
            {
                for (int z = -offset; z < offset; z++)
                {
                    AddTile(Node, x, z);
                }
            }

            for (int x = lastOffset; x < offset; x++)
            {
                for (int z = -offset; z < offset; z++)
                {
                    AddTile(Node, x, z);
                }
            }

            for (int x = -lastOffset; x < lastOffset; x++)
            {
                for (int z = -offset; z < -lastOffset; z++)
                {
                    AddTile(Node, x, z);
                }
            }

            for (int x = -lastOffset; x < lastOffset; x++)
            {
                for (int z = lastOffset; z < offset; z++)
                {
                    AddTile(Node, x, z);
                }
            }
        }

        public override void OnSerialize(Urho.Resources.IComponentSerializer serializer)
        {
            base.OnSerialize(serializer);

            //serializer.Serialize("Seed", m_Seed);
            //serializer.Serialize("CurrentSize", m_CurrentSize);
            //serializer.Serialize("BuildableTiles", m_BuildableTiles);
            //serializer.Serialize("OccupiedTiles", m_OccupiedTiles);
        }

        public override void OnDeserialize(Urho.Resources.IComponentDeserializer deserializer)
        {
            //int seed = deserializer.Deserialize<int>("Seed");
            //int size = deserializer.Deserialize<int>("CurrentSize");
            //
            //int buildableTiles = deserializer.Deserialize<int>("BuildableTiles");
            //int occupiedTiles = deserializer.Deserialize<int>("OccupiedTiles");

//             var children = Node.GetChildrenWithTag("tile");
//             foreach (var c in children)
//             {
//                 Node.RemoveChild(c);
//             }

//             m_CurrentSize = INITIAL_SIZE;
//             m_BuildableTiles = 0;
//             m_OccupiedTiles = 0;

            //m_CurrentSize = seed;
            //m_BuildableTiles = buildableTiles;
            //m_OccupiedTiles = occupiedTiles;
            //
            //Initialize(seed);

            //OnAttachedToNode(Node);
            //while (m_CurrentSize < size)
            //{
            //    Extend();
            //}
            //
            //Debug.Assert(m_CurrentSize == size);
            //Debug.Assert(m_BuildableTiles == buildableTiles);
            //Debug.Assert(m_OccupiedTiles == occupiedTiles);

            base.OnDeserialize(deserializer);
        }

//         public override bool SaveXml(Urho.Resources.XmlElement dest)
//         {
//             return base.SaveXml(dest);
//         }
// 
//         public override bool LoadXml(Urho.Resources.XmlElement source, bool setInstanceDefault)
//         {
//             return base.LoadXml(source, setInstanceDefault);
//         }
// 
//         public override bool Save(Urho.IO.File dest)
//         {
//             return base.Save(dest);
//         }
// 
//         public override bool Save(Urho.MemoryBuffer dest)
//         {
//             return base.Save(dest);
//         }
// 
//         public override bool Load(Urho.IO.File source, bool setInstanceDefault)
//         {
//             return base.Load(source, setInstanceDefault);
//         }
// 
//         public override bool Load(Urho.MemoryBuffer source, bool setInstanceDefault)
//         {
//             return base.Load(source, setInstanceDefault);
//         }
// 
//         public override void OnSceneSet(Urho.Scene scene)
//         {
//             base.OnSceneSet(scene);
//         }
// 
//         public override void OnNodeSetEnabled()
//         {
//             base.OnNodeSetEnabled();
//         }
// 
//         public override void OnSetEnabled()
//         {
//             base.OnSetEnabled();
//         }
    }
}
