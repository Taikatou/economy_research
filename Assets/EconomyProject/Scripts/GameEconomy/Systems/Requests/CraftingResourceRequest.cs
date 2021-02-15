using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class CraftingResourceRequest
    {
        public readonly float CreationTime;
        private bool _resourceAdded;
        public CraftingResources Resource { get; }
        public CraftingInventory Inventory { get; }
        public int Number { get; set; }
        public int Price { get; set; }
		public Sprite Icon;
        public int Reward => GetReward(Price);

        public int GetReward(int newPrice)
        {
            return Number * newPrice;
        }

		public CraftingResourceRequest(CraftingResources resource, CraftingInventory inventory, int price, Sprite icon)
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

        private static float[] GetItemSenses(List<CraftingResourceRequest> request, int index)
        {
            var output = new float [3];
            if (index < request.Count)
            {
                output[0] = (float) request[index].Resource;
                output[1] = request[index].Price;
                output[2] = request[index].Number;
            }
            return output;
        }

        public static float[] GetSenses(List<CraftingResourceRequest> craftingRequests, int number)
        {
            var output = new float[SensorCount];
            for (var i = 0; i < number; i++)
            {
                var index = i * 3;
                var sense = GetItemSenses(craftingRequests, 1);
                for (var j = 0; j < sense.Length; j++)
                {
                    output[index + j] = sense[j];
                }
            }

            return output;
        }
        
        public const int SensorCount = 15;
    }
}
