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
        int[] ResourceValues;

        public IdlePlayerResources()
        {
            Initialize();
        }

        private void Initialize()
        {
            ResourceValues = new int[(int)IdlePlayerResourceType.Count];
        }

        public int GetResourceValue(IdlePlayerResourceType resourceType)
        {
            return ResourceValues[(int)resourceType];
        }

        public void SetResourceValue(IdlePlayerResourceType resourceType, int value)
        {
            ResourceValues[(int)resourceType] = value;
        }

        public void AddResourceValue(IdlePlayerResourceType resourceType, int value)
        {
            ResourceValues[(int)resourceType] += value;
        }

        public void RemoveResourceValue(IdlePlayerResourceType resourceType, int value)
        {
            ResourceValues[(int)resourceType] -= value;
        }
    }
}
