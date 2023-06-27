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
        public float TakenTime;
        public float CompletedTime;
        
        public float CreationTime { get; }
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

        public static ObsData[] GetObservations(Dictionary<ECraftingResources, CraftingResourceRequest> craftingRequests)
        {
            var output = new ObsData[SensorCount];
            var counter = 0;
            foreach (var item in craftingRequests)
            {
                var resource = item.Value?.Resource ?? ECraftingResources.Nothing;
                var price = item.Value?.Price ?? 0;
                var number = item.Value?.Number ?? 0;
                output[counter++] = new CategoricalObsData<ECraftingResources>(resource)
                {
                    Name="resource",
                };
                output[counter++] = new SingleObsData
                {
                    data=price,
                    Name="itemPrice"
                };
                output[counter++] = new SingleObsData
                {
                    data=number / 10,
                    Name="itemNumber"
                };
                if (counter >= SensorCount)
                {
                    break;
                }
            }

            while(counter < SensorCount)
            {
                output[counter++] = new SingleObsData { Name="resource"};
                output[counter++] = new SingleObsData { Name="itemPrice"};
                output[counter++] = new SingleObsData { Name="itemNumber"};
            }

            return output;
        }

        private static int ItemCount => Enum.GetValues(typeof(ECraftingResources)).Length;

        public static int SensorCount => ItemCount * 3;
    }
}
