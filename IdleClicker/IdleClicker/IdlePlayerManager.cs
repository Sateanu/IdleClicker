using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;

namespace IdleClicker
{
    class IdlePlayerManager
    {
        private static IdlePlayerManager IdlePlayerManagerInstance;

        public static IdlePlayerManager Instance
        {
            get
            {
                if(IdlePlayerManagerInstance == null)
                {
                    IdlePlayerManagerInstance = new IdlePlayerManager();
                }

                return IdlePlayerManagerInstance;
            }
        }


        IdlePlayerResources PlayerResources;
        List<Building> Buildings;


        public IdlePlayerManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            PlayerResources = new IdlePlayerResources();
            Buildings = new List<Building>();
        }

        public float GetResourceValue(IdlePlayerResourceType resourceType)
        {
            return PlayerResources.GetResourceValue(resourceType);
        }

        public void SetResourceValue(IdlePlayerResourceType resourceType, float value)
        {
            PlayerResources.SetResourceValue(resourceType, value);
        }

        public void AddResourceValue(IdlePlayerResourceType resourceType, float value)
        {
            PlayerResources.AddResourceValue(resourceType, value);
        }

        public void RemoveResourceValue(IdlePlayerResourceType resourceType, float value)
        {
            PlayerResources.RemoveResourceValue(resourceType, value);
        }
    }
}
