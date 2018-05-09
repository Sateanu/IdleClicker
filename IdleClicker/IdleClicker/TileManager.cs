﻿using System;
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

        private Random m_RndGen = new Random();

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
                tileComp.AddDebris(new DebrisProperties());
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
