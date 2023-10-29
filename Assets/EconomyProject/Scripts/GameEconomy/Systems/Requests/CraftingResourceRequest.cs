using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using Unity.MLAgents.Sensors;
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
        
        public static void GetObservations(Dictionary<ECraftingResources, List<CraftingResourceRequest>> craftingRequests, BufferSensorComponent requestBuffer)
        {
            foreach (var item in craftingRequests)
            {
                foreach (var resource in item.Value)
                {
                    var output = new ObsData[3];
                    ObserveData(resource, output);
                    requestBuffer.AppendObservation(ObsData.GetEnumerableData(output));
                }
            }
        }

        private static void ObserveData(CraftingResourceRequest value, ObsData[] output)
        {
            var resource = value?.Resource ?? ECraftingResources.Nothing;
            var price = value?.Price ?? 0;
            var number = value?.Number ?? 0;
            output[0] = new CategoricalObsData<ECraftingResources>(resource)
            {
                Name="resource",
            };
            output[1] = new SingleObsData
            {
                data=price,
                Name="itemPrice"
            };
            output[2] = new SingleObsData
            {
                data=number / 10,
                Name="itemNumber"
            };
            
        }

        public static void GetObservations(Dictionary<ECraftingResources, CraftingResourceRequest> craftingRequests, BufferSensorComponent requestBuffer)
        {
            var counter = 0;
            foreach (var item in craftingRequests)
            {
                var output = new ObsData[3];
                ObserveData(item.Value, output);
                requestBuffer.AppendObservation(ObsData.GetEnumerableData(output));
            }
        }

        private static int ItemCount => Enum.GetValues(typeof(ECraftingResources)).Length;

        public static int SensorCount => ItemCount * 3;
    }
}
