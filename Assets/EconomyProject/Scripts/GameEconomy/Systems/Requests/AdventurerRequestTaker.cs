using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.iOS;

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

        public static float[] GetSenses(Dictionary<ECraftingResources, TakenCraftingResourceRequest> dictionary, ECraftingResources key)
        {
            var output = new float[4];
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
    }
    
    public delegate void OnResources();
    
    public class AdventurerRequestTaker : RequestTaker
    {
        public EconomyWallet wallet;
        public RequestSystem requestSystem;
        public CraftingRequestRecord requestRecord;

        private Dictionary<ECraftingResources, TakenCraftingResourceRequest> CurrentRequestData { get; set; }

        //TODO THIS SHOULDN'T BE HARD LIMIT
        public override IEnumerable<CraftingResourceRequest> GetItemList() => requestRecord != null? requestRecord.GetCurrentRequests(this) : new List<CraftingResourceRequest>();
        
        public void Start()
        {
            CurrentRequestData = new Dictionary<ECraftingResources, TakenCraftingResourceRequest>();
        }
        
        public override void TakeRequest(CraftingResourceRequest request)
        {
            if (request == null || request.Inventory == null)
            {
                Debug.Log("FAILURE");
            }
            requestSystem.TakeRequest(this, request);
        }

        public override void CompleteRequest(int reward)
        {
            wallet.EarnMoney(reward);
        }

        public void CheckItemAdd(ECraftingResources resource, int amount, OnResources onResourceAdd=null, OnResources onResourceComplete=null)
        {
            var itemList = GetItemList();
            foreach (var item in itemList)
            {
                Debug.Log(item.Resource);
            }
            Debug.Log("Resource: " + resource + " - amount: " + amount);
            
            
            // Check if we have current request for this resource
            if (TrainingConfig.AdventurerNoRequestMenu && !CurrentRequestData.ContainsKey(resource))
            {
                var currentRequests = requestSystem.GetAllCraftingRequests();
                CraftingResourceRequest craftingResourceRequest = null;
                var greatestPrice = 0;
                foreach (var request in currentRequests)
                {
                    if (request.Resource == resource)
                    {
                        if (request.Price > greatestPrice)
                        {
                            craftingResourceRequest = request;
                            greatestPrice = request.Price;
                        }
                    }
                }

                if (craftingResourceRequest != null)
                {
                    TakeRequest(craftingResourceRequest);
                }
            }
            
            var foundRequest = itemList.FirstOrDefault(i => i.Resource == resource);
            if (foundRequest != null)
            {
                if (!CurrentRequestData.ContainsKey(resource))
                {
                    var craftingRequest = new TakenCraftingResourceRequest {Request = foundRequest, CurrentAmount = amount};
                    CurrentRequestData.Add(resource, craftingRequest);
                }
                else
                {
                    CurrentRequestData[resource].AddAmount(amount);
                }
                if (CurrentRequestData[resource].Complete)
                {
                    requestRecord.CompleteRequest(this, CurrentRequestData[resource].Request);
                    CurrentRequestData.Remove(resource);
                    onResourceComplete?.Invoke();
                }
                else
                {
                    onResourceAdd?.Invoke();
                }
                requestSystem.Refresh();
            }
        }

        public int GetCurrentStock(ECraftingResources resource)
        {
            var amount = 0;
            if (CurrentRequestData.ContainsKey(resource))
            {
                amount = CurrentRequestData[resource].CurrentAmount;
            }
            return amount;
        }

        public void GetCurrentRequestData(BufferSensorComponent requestTakenBufferComponent)
        {
            var resources = CraftingUtils.GetCraftingResources();
            foreach (var r in resources)
            {
                var senses = TakenCraftingResourceRequest.GetSenses(CurrentRequestData, r);
                requestTakenBufferComponent.AppendObservation(senses);
            }
        }
    }
}
