using Urho;

namespace IdleClicker
{
    class TileManager : Component
    {
        public override void OnAttachedToNode(Node node)
        {
            for (int x = -4; x < 4; x++)
            {
                for (int z = -4; z < 4; z++)
                {
                    var childTile = node.CreateChild("" + x + "_" + z);
                    childTile.Position = new Vector3(x, 0, z);
                    childTile.CreateComponent<BuildingTile>();
                }
            }
        }
    }
}
