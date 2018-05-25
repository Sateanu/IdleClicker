using System;
using System.Collections.Generic;
using System.Diagnostics;
using Urho;

namespace IdleClicker
{
    class TileManager : Component
    {
        const int INITIAL_SIZE = 9;
        int currentSize = INITIAL_SIZE;

        int buildableTiles = 0;
        int occupiedTiles = 0;

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
            childTile.Position = new Vector3(x, 0, z);

            var tileComp = childTile.CreateComponent<BuildingTile>();
            bool isBuildable = m_RndGen.NextDouble() < BUILDABLE_RATIO;

            tileComp.IsBuildable = isBuildable;
            tileComp.Manager = this;

            if (isBuildable)
                buildableTiles++;
            else
                tileComp.AddDebris(m_DebrisData.GetNewDebris());
        }

        public override void OnAttachedToNode(Node node)
        {
            int offset = currentSize / 2;

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
            occupiedTiles++;

            if ((float)occupiedTiles / buildableTiles > EXTEND_RATIO)
                Extend();
        }

        public void DeleteBuilding()
        {
            occupiedTiles--;
        }

        private void Extend()
        {
            int lastOffset = currentSize / 2;
            int offset = currentSize;
            currentSize *= 2;

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
    }
}
