using System;
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
         : this((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)
        {}

        public TileManager(int seed)
        {
            m_Seed = seed;
            m_RndGen = new Random(m_Seed);
            m_DebrisData = new DebrisData(m_Seed);
        }

        private void AddTile(Node node, int x, int z)
        {
            var childTile = node.CreateChild("" + x + "_" + z);
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
