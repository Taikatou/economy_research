using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class TakenCraftingResourceRequest
    {
        public CraftingResourceRequest Request;
        public int CurrentAmount;

        public void AddAmount(int amount)
        {
            CurrentAmount += amount;
        }

        public static float[] GetSenses(Dictionary<CraftingResources, TakenCraftingResourceRequest> dictionary, CraftingResources key)
        {
            var output = new float [SensorCount];
            if (dictionary.ContainsKey(key))
            {
                output[0] = (float) dictionary[key].Request.Resource;
                output[1] = dictionary[key].Request.Price;
                output[2] = dictionary[key].Request.Number;
                output[3] = dictionary[key].CurrentAmount;
            }
            return output;
        }

        public bool Complete => Request.Number <= CurrentAmount;
        
        public const int SensorCount = 4;
    }
    
    public class AdventurerRequestTaker : RequestTaker
    {
        public EconomyWallet wallet;
        public RequestSystem requestSystem;
        public CraftingRequestRecord requestRecord;

        private Dictionary<CraftingResources, TakenCraftingResourceRequest> _currentRequestData;

        public override List<CraftingResourceRequest> GetItemList() => requestRecord.GetCurrentRequests(this);
        
        public void Start()
        {
            _currentRequestData = new Dictionary<CraftingResources, TakenCraftingResourceRequest>();
        }
        
        public override void TakeRequest(CraftingResourceRequest request)
        {
            requestSystem.TakeRequest(this, request);
        }

        public override void CompleteRequest(int reward)
        {
            wallet.EarnMoney(reward);
        }

        public void CheckItemAdd(CraftingResources resource, int amount)
        {
            var itemList = GetItemList();
            foreach (var item in itemList)
            {
                Debug.Log(item.Resource);
            }
            Debug.Log("Resource: " + resource);
            var foundRequest = itemList.FirstOrDefault(i => i.Resource == resource);
            
            // Check if we have current request for this resource
            if (foundRequest != null)
            {
                if (!_currentRequestData.ContainsKey(resource))
                {
                    var craftingRequest = new TakenCraftingResourceRequest {Request = foundRequest, CurrentAmount = amount};
                    _currentRequestData.Add(resource, craftingRequest);
                }
                else
                {
                    _currentRequestData[resource].AddAmount(amount);
                }
                if (_currentRequestData[resource].Complete)
                {
                    requestRecord.CompleteRequest(this, _currentRequestData[resource].Request);
                    _currentRequestData.Remove(resource);
                }
                requestSystem.Refresh();
            }
        }

        public int GetCurrentStock(CraftingResources resource)
        {
            var amount = 0;
            if (_currentRequestData.ContainsKey(resource))
            {
                amount = _currentRequestData[resource].CurrentAmount;
            }
            return amount;
        }

        public float[] GetSenses()
        {
            var resources = CraftingUtils.GetCraftingResources();
            var outputSize =  resources.Count * TakenCraftingResourceRequest.SensorCount;
            var output = new float[outputSize];
            for (var i = 0; i < resources.Count; i++ )
            {
                var senses = TakenCraftingResourceRequest.GetSenses(_currentRequestData, resources[i]);
                for (var j = 0; j < senses.Length; j++)
                {
                    var index = i*TakenCraftingResourceRequest.SensorCount + j;
                    output[index] = senses[j];
                }
            }

            return output;
        }
    }
}
