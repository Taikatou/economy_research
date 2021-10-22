using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class RequestSystem : LastUpdate, IShopSense, ISetup
    {
        public CraftingInventory testCraftingInventory;
        public bool testRequests = false;
        private static int MAX_PRICE = 50;
        
        public int maxRequests = 2;
        public CraftingRequestRecord craftingRequestRecord;

		public Sprite[] iconResources;

		private Dictionary<CraftingInventory, Dictionary<ECraftingResources, CraftingResourceRequest>> _craftingRequests;
        private Dictionary<CraftingResourceRequest, EconomyWallet> _requestWallets;

		public Dictionary<ECraftingResources, int> defaultResourcePrices = new Dictionary<ECraftingResources, int> {
			{ECraftingResources.Wood, 5},
			{ECraftingResources.Metal, 6},
			{ECraftingResources.Gem, 7},
			{ECraftingResources.DragonScale, 8}
		};

        public void Setup()
        {
            _requestWallets.Clear();
            _craftingRequests.Clear();
        }

		public Dictionary<AgentType, int> StartMoney => new Dictionary<AgentType, int>
		{
			{AgentType.Adventurer, 100},
			{AgentType.Shop, 1000},
		};

		public List<CraftingResourceRequest> GetAllCraftingRequests()
        {
            var returnList = new List<CraftingResourceRequest>();
            foreach (var entry in _craftingRequests)
            {
                foreach (var entry2 in entry.Value)
                {
                    if (entry2.Value != null)
                    {
                        returnList.Add(entry2.Value);   
                    }
                }
            }

            return returnList;
        }
        
        public List<CraftingResourceRequest> GetAllCraftingRequests(CraftingInventory inventory)
        {
            var returnList = new List<CraftingResourceRequest>();
            if (_craftingRequests.ContainsKey(inventory))
            {
                foreach (var entry2 in _craftingRequests[inventory])
                {
                    returnList.Add(entry2.Value);
                }
            }

            return returnList;
        }

        private int GetRequestNumber(CraftingInventory inventory, ECraftingResources resources)
        {
            if (_craftingRequests.ContainsKey(inventory))
            {
                if (_craftingRequests[inventory].ContainsKey(resources))
                {
                    return _craftingRequests[inventory][resources].Number;
                }
            }

            return 0;
        }
        
        public int GetRequestNumber(CraftingInventory inventory)
        {
            if (_craftingRequests.ContainsKey(inventory))
            {
                return _craftingRequests[inventory].Count;
            }

            return 0;
        }

		public Sprite GetIconByResource(ECraftingResources resource)
		{
			switch (resource)
			{
				case ECraftingResources.Wood:
					return iconResources[0];
				case ECraftingResources.Metal:
					return iconResources[1];
				case ECraftingResources.Gem:
					return iconResources[2];
				case ECraftingResources.DragonScale:
					return iconResources[3];
				default:
					Debug.Log("Wrong Crafting resource : " + resource);
					return iconResources[0];
			}
		}

        public void Start()
        {
			ResetRequestSystem();
            TestRequests();
		}

        private void TestRequests()
        {
            if (testRequests)
            {
                _craftingRequests.Add(testCraftingInventory, new Dictionary<ECraftingResources, CraftingResourceRequest>
                {
                    { ECraftingResources.Gem, new CraftingResourceRequest(ECraftingResources.Gem, testCraftingInventory, 4, null)}
                });
                Refresh();
            }
        }

		public void ResetRequestSystem()
		{
			_craftingRequests = new Dictionary<CraftingInventory, Dictionary<ECraftingResources, CraftingResourceRequest>>();
			_requestWallets = new Dictionary<CraftingResourceRequest, EconomyWallet>();
			Refresh();
		}
        
        public void MakeRequest(ECraftingResources resources, CraftingInventory inventory, EconomyWallet wallet)
        {
            void CheckExchange(CraftingResourceRequest request)
            {
                if(request.Price <= wallet.Money)
                {
                    if (_craftingRequests[inventory].ContainsKey(resources))
                    {
                        _craftingRequests[inventory][resources].Number++;
                    }
                    else
                    {
                        _craftingRequests[inventory].Add(resources, request);
                    }
                    wallet.SpendMoney(request.Price);
                }
            }
            CheckInventory(inventory);

            var requestNumber = GetRequestNumber(inventory, resources);
            var canRequest = requestNumber < 10;

			if (canRequest)
            {
                var containsKey = _craftingRequests[inventory].ContainsKey(resources);

				if (!containsKey)
                {
					Sprite icon = GetIconByResource(resources);
                    var newResource = new CraftingResourceRequest(resources, inventory, defaultResourcePrices[resources], icon);
                    _requestWallets.Add(newResource, wallet);
                    CheckExchange(newResource);
                }
                else
                {
                    CheckExchange(_craftingRequests[inventory][resources]);
                }
            }

            Refresh();
        }
        private void CheckInventory(CraftingInventory inventory)
        {
            if (!_craftingRequests.ContainsKey(inventory))
            {
                _craftingRequests.Add(inventory, new Dictionary<ECraftingResources, CraftingResourceRequest>());
            }
        }

        public void ChangePrice(ECraftingResources resources, CraftingInventory inventory, EconomyWallet wallet, int change)
        {
            CheckInventory(inventory);
            var craftingInventory = _craftingRequests[inventory];
            var containsKey = craftingInventory.ContainsKey(resources);
            if (containsKey)
            {
                if(craftingInventory.ContainsKey(resources))
                {
                    var newPrice = craftingInventory[resources].Price + change;
                    if (newPrice > 0 && newPrice < MAX_PRICE)
                    {
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
            }
            Refresh();
        }

        public void RemoveRequest(ECraftingResources resource, CraftingInventory inventory)
        {
            if (_craftingRequests.ContainsKey(inventory))
            {
                if (_craftingRequests[inventory].ContainsKey(resource))
                {
                    var request = _craftingRequests[inventory][resource];
                    if (_requestWallets.ContainsKey(request))
                    {
                        _requestWallets[request].EarnMoney(request.Reward);   
                        _requestWallets.Remove(request);
                    }

                    if (_craftingRequests.ContainsKey(inventory))
                    {
                        _craftingRequests[inventory].Remove(resource);
                    }

                    Refresh();
                }
            }
        }

        public void TakeRequest(RequestTaker requestTaker, CraftingResourceRequest takeRequest)
        {
            var inventory = takeRequest.Inventory;
            
            var containsResource = _craftingRequests[inventory].ContainsKey(takeRequest.Resource);
            Debug.Log("Contains resource " + containsResource);

            var currentRequests = craftingRequestRecord.GetCurrentRequests(requestTaker);
            var validRequests = currentRequests.Length < maxRequests;
            if (containsResource && validRequests)
            {
                Debug.Log("Took request");
                craftingRequestRecord.AddRequest(requestTaker, takeRequest);
                _craftingRequests[inventory].Remove(takeRequest.Resource);
            }

            Refresh();
        }

        public ObsData[] GetObservations(AdventurerAgent agent)
        {
            var craftingRequests = GetAllCraftingRequests();
            
            return CraftingResourceRequest.GetObservations(craftingRequests);
        }

        public ObsData[] GetObservations(ShopAgent agent)
        {
            var craftingRequests = GetAllCraftingRequests(agent.craftingInventory);
            return CraftingResourceRequest.GetObservations(craftingRequests);
        }

        private void UpdateRemove()
        {
            var currentTime = Time.time;
            var toRemove = new List<Tuple<ECraftingResources, CraftingInventory>>();
            var removedRequests = false;
            foreach (var entry in _craftingRequests)
            {
                foreach (var item in entry.Value)
                {
                    if (Math.Abs(item.Value.CreationTime - currentTime) > 30)
                    {
                        var r = new Tuple<ECraftingResources, CraftingInventory>(item.Value.Resource, entry.Key);
                        toRemove.Add(r);
                    }
                }
            }

            foreach (var item in toRemove)
            {
                removedRequests = true;
                RemoveRequest(item.Item1, item.Item2);
            }

            if (removedRequests)
            {
                Debug.Log(" -  -  -    Removed Requests   -  -  - ");
            }
        }

        public void Update()
        {
            if (SystemTraining.removeRequestTime)
            {
                UpdateRemove();
            }
        }
    }
}
