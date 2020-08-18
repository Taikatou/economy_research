using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;

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
    
    public class AdventurerRequestTaker : RequestTaker
    {
        public RequestSystem requestSystem;
        public CraftingRequestRecord requestRecord;

        private Dictionary<CraftingResources, TakenCraftingResourceRequest> _currentActiveRequests;

        public override List<CraftingResourceRequest> ItemList => requestRecord.GetCurrentRequests(this);
        
        public void Start()
        {
            _currentActiveRequests = new Dictionary<CraftingResources, TakenCraftingResourceRequest>();
        }
        
        public override void TakeRequest(CraftingResourceRequest request)
        {
            requestSystem.TakeRequest(this, request);
        }

        public void CheckItemAdd(CraftingResources resource, int amount)
        {
            var found = _currentActiveRequests.ContainsKey(resource);
            if (!found)
            {
                var foundRequest = ItemList.FirstOrDefault(i => i.Resource == resource);
                if (foundRequest != null)
                {
                    var craftingRequest = new TakenCraftingResourceRequest {Request = foundRequest, CurrentAmount = 0};
                    _currentActiveRequests.Add(resource, craftingRequest);
                    found = true;
                }
            }
            
            if(found)
            {
                _currentActiveRequests[resource].AddAmount(amount);
                if (_currentActiveRequests[resource].Complete)
                {
                    requestRecord.CompleteRequest(this, _currentActiveRequests[resource].Request);
                    _currentActiveRequests.Remove(resource);
                }
            }
        }
    }
}
