using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    // 
    public class CraftingResourceRequest
    {
        public readonly float CreationTime;
        private bool _resourceAdded;
        public ECraftingResources Resource { get; }
        public CraftingInventory Inventory { get; }
        public int Number { get; set; }
        public int Price { get; set; }
		public Sprite Icon;
        public int Reward => GetReward(Price);

        public int GetReward(int newPrice)
        {
            return Number * newPrice;
        }

		public CraftingResourceRequest(ECraftingResources resource, CraftingInventory inventory, int price, Sprite icon)
        {
            Number = 1;
            Resource = resource;
            Inventory = inventory;
            Price = price;
			Icon = icon;

			CreationTime = Time.time;
        }

        public void TransferResource()
        {
            if (!_resourceAdded)
            {
                _resourceAdded = true;
                Inventory.AddResource(Resource, Number);
            }
        }

        public static ObsData[] GetObservations(IEnumerable<CraftingResourceRequest> craftingRequests)
        {
            var output = new ObsData[SensorCount];
            var counter = 0;
            foreach (var item in craftingRequests)
            {
                output[counter++] = new ObsData { data=(float) item.Resource, name="resource"};
                output[counter++] = new ObsData { data=item.Price, name="itemPrice"};
                output[counter++] = new ObsData { data=item.Number, name="itemNumber"};
                if (counter >= SensorCount)
                {
                    break;
                }
            }

            while(counter < SensorCount)
            {
                output[counter++] = new ObsData { name="resource"};
                output[counter++] = new ObsData { name="itemPrice"};
                output[counter++] = new ObsData { name="itemNumber"};
            }

            return output;
        }

        private static int ItemCount => Enum.GetValues(typeof(ECraftingResources)).Length;

        public static int SensorCount => ItemCount * 3;
    }
}
