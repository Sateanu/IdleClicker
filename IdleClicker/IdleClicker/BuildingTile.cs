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
        public Building Building { get; set; }

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
        }

        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);

            if(Building!=null)
            {
                Building.Update(timeStep);
            }
        }
    }
}
