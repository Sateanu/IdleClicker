using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;

namespace IdleClicker
{
    public enum IdlePlayerResourceType
    {
        Food,
        Wood,
        Stone,
        Gold,

        Count
    }

    class IdlePlayerResources
    {
        float[] ResourceValues;

        public IdlePlayerResources()
        {
            Initialize();
        }

        private void Initialize()
        {
            ResourceValues = new float[(int)IdlePlayerResourceType.Count];
        }

        public float GetResourceValue(IdlePlayerResourceType resourceType)
        {
            return ResourceValues[(int)resourceType];
        }

        public void SetResourceValue(IdlePlayerResourceType resourceType, float value)
        {
            ResourceValues[(int)resourceType] = value;
        }

        public void AddResourceValue(IdlePlayerResourceType resourceType, float value)
        {
            ResourceValues[(int)resourceType] += value;
        }

        public void RemoveResourceValue(IdlePlayerResourceType resourceType, float value)
        {
            ResourceValues[(int)resourceType] -= value;
        }
    }
}
