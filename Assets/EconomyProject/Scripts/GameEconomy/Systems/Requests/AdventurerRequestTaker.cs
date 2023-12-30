using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
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

        public bool Complete => Request.Number <= CurrentAmount;
    }
    
    public delegate void OnResources<T>(T agent) where T : Agent;
    
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
            wallet.EarnMoney(reward, true);
        }

        public bool CheckItemAdd(BaseAdventurerAgent agent, ECraftingResources resource, int amount, OnResources<BaseAdventurerAgent> onResourceAdd=null, OnResources<BaseAdventurerAgent> onResourceComplete=null)
        {
            var itemList = GetItemList();
            if (CurrentRequestData == null)
            {
                return false;
            }
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
                    Debug.Log("Request Completed");
                    onResourceComplete?.Invoke(agent);
                }
                else
                {
                    onResourceAdd?.Invoke(agent);
                }
                requestSystem.Refresh();
                return true;
            }

            return false;
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

        public float[] GetCurrentRequestData()
        {
            var data = new float[4];
            var resources = CraftingUtils.GetCraftingResources();
            var index = 0;
            foreach (var r in resources)
            {
                if (CurrentRequestData.ContainsKey(r))
                {
                    data[index] = 1;
                }
                index++;
            }

            return data;
        }
    }
}
