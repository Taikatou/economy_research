using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class RequestSystem : LastUpdate
    {
        public int maxRequests = 2;
        public CraftingRequestRecord craftingRequestRecord;

        private Dictionary<CraftingInventory, Dictionary<CraftingResources, CraftingResourceRequest>> _craftingNumber;
        private Dictionary<CraftingResourceRequest, EconomyWallet> _requestWallets;

        public List<CraftingResourceRequest> GetAllCraftingRequests()
        {
            var returnList = new List<CraftingResourceRequest>();
            foreach (var entry in _craftingNumber)
            {
                foreach (var entry2 in entry.Value)
                {
                    returnList.Add(entry2.Value);
                }
            }

            return returnList;
        }

        private int GetRequestNumber(CraftingInventory inventory, CraftingResources resources)
        {
            if (_craftingNumber.ContainsKey(inventory))
            {
                if (_craftingNumber[inventory].ContainsKey(resources))
                {
                    return _craftingNumber[inventory][resources].Number;
                }
            }

            return 0;
        }
        
        public int GetRequestNumber(CraftingInventory inventory)
        {
            if (_craftingNumber.ContainsKey(inventory))
            {
                return _craftingNumber[inventory].Count;
            }

            return 0;
        }

        private void Start()
        {
            _craftingNumber = new Dictionary<CraftingInventory, Dictionary<CraftingResources, CraftingResourceRequest>>();
            _requestWallets = new Dictionary<CraftingResourceRequest, EconomyWallet>();
            Refresh();
        }
        
        public void MakeRequest(CraftingResources resources, CraftingInventory inventory, EconomyWallet wallet)
        {
            void CheckExchange(CraftingResourceRequest request)
            {
                if(request.Price <= wallet.Money)
                {
                    if (_craftingNumber[inventory].ContainsKey(resources))
                    {
                        _craftingNumber[inventory][resources].Number++;
                    }
                    else
                    {
                        _craftingNumber[inventory].Add(resources, request);   
                    }
                    wallet.SpendMoney(request.Price);
                }
            }
            CheckInventory(inventory);

            var requestNumber = GetRequestNumber(inventory, resources);
            var canRequest = requestNumber < 5;

            if (canRequest)
            {
                var containsKey = _craftingNumber[inventory].ContainsKey(resources);
                if (!containsKey)
                {
                    var newResource = new CraftingResourceRequest(resources, inventory);
                    _requestWallets.Add(newResource, wallet);
                    CheckExchange(newResource);
                }
                else
                {
                    CheckExchange(_craftingNumber[inventory][resources]);
                }
            }

            Refresh();
        }
        private void CheckInventory(CraftingInventory inventory)
        {
            if (!_craftingNumber.ContainsKey(inventory))
            {
                _craftingNumber.Add(inventory, new Dictionary<CraftingResources, CraftingResourceRequest>());
            }
        }

        public void ChangePrice(CraftingResources resources, CraftingInventory inventory, EconomyWallet wallet, int change)
        {
            CheckInventory(inventory);
            var craftingInventory = _craftingNumber[inventory];
            var containsKey = craftingInventory.ContainsKey(resources);
            if (containsKey)
            {
                if(craftingInventory.ContainsKey(resources))
                {
                    var newPrice = craftingInventory[resources].Price + change;
                    var newReward = craftingInventory[resources].GetReward(newPrice);

                    var rewardDifference = newReward - craftingInventory[resources].Reward;
                    var validTransaction = false;
                    if (rewardDifference > 0)
                    {
                        if (rewardDifference <= wallet.Money)
                        {
                            wallet.SpendMoney(rewardDifference);
                            validTransaction = true;
                        }
                    }
                    else
                    {
                        wallet.EarnMoney(Mathf.Abs(rewardDifference));
                        validTransaction = true;
                    }

                    if (validTransaction)
                    {
                        craftingInventory[resources].Price = newPrice;
                    }
                }
            }
            Refresh();
        }

        public void RemoveRequest(CraftingResources resource, CraftingInventory inventory)
        {
            if (_craftingNumber.ContainsKey(inventory))
            {
                if (_craftingNumber[inventory].ContainsKey(resource))
                {
                    var request = _craftingNumber[inventory][resource];
                    _requestWallets[request].EarnMoney(request.Reward);
                    _craftingNumber[inventory].Remove(resource);
                    _requestWallets.Remove(request);
            
                    Refresh();
                }
            }
        }

        public void TakeRequest(RequestTaker requestTaker, CraftingResourceRequest takeRequest)
        {
            var craftingNumber = _craftingNumber[takeRequest.Inventory];
            var containsResource = craftingNumber.ContainsKey(takeRequest.Resource);
            Debug.Log("Contains resource " + containsResource);

            var currentRequests = craftingRequestRecord.GetCurrentRequests(requestTaker);
            var validRequests = currentRequests.Count < maxRequests;
            if (containsResource && validRequests)
            {
                Debug.Log("Took request");
                craftingRequestRecord.AddRequest(requestTaker, takeRequest);
                craftingNumber.Remove(takeRequest.Resource);
            }

            Refresh();
        }

        public void CompleteRequest(RequestTaker taker, CraftingResourceRequest request)
        {
            craftingRequestRecord.CompleteRequest(taker, request);
            if (_craftingNumber.ContainsKey(request.Inventory))
            {
                if (_craftingNumber[request.Inventory].ContainsKey(request.Resource))
                {
                    _craftingNumber[request.Inventory].Remove(request.Resource);
                    var count = _craftingNumber[request.Inventory].Count;
                    if (count == 0)
                    {
                        _craftingNumber.Remove(request.Inventory);
                    }
                }
            }
        }
    }
}
