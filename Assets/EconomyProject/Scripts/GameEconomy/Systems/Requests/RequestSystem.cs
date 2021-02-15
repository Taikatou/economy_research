using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;
using System.ComponentModel;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class RequestSystem : LastUpdate, IAdventureSense, IShopSense
    {
        public int maxRequests = 2;
        public CraftingRequestRecord craftingRequestRecord;

		public Sprite[] iconResources;

		private Dictionary<CraftingInventory, Dictionary<CraftingResources, CraftingResourceRequest>> _craftingRequests;
        private Dictionary<CraftingResourceRequest, EconomyWallet> _requestWallets;

		public Dictionary<CraftingResources, int> defaultResourcePrices = new Dictionary<CraftingResources, int> {
			{CraftingResources.Wood, 5},
			{CraftingResources.Metal, 6},
			{CraftingResources.Gem, 7},
			{CraftingResources.DragonScale, 8}
		};

		public Dictionary<AgentType, int> _startMoney = new Dictionary<AgentType, int>
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
                    returnList.Add(entry2.Value);
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

        private int GetRequestNumber(CraftingInventory inventory, CraftingResources resources)
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

		public Sprite GetIconByResource(CraftingResources resource)
		{
			switch (resource)
			{
				case CraftingResources.Wood:
					return iconResources[0];
				case CraftingResources.Metal:
					return iconResources[1];
				case CraftingResources.Gem:
					return iconResources[2];
				case CraftingResources.DragonScale:
					return iconResources[3];
				default:
					Debug.Log("Wrong Crafting resource : " + resource);
					return iconResources[0];
			}
		}

        public void Start()
        {
			ResetRequestSystem();
		}

		public void ResetRequestSystem()
		{
			_craftingRequests = new Dictionary<CraftingInventory, Dictionary<CraftingResources, CraftingResourceRequest>>();
			_requestWallets = new Dictionary<CraftingResourceRequest, EconomyWallet>();
			Refresh();
		}
        
        public void MakeRequest(CraftingResources resources, CraftingInventory inventory, EconomyWallet wallet)
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
                _craftingRequests.Add(inventory, new Dictionary<CraftingResources, CraftingResourceRequest>());
            }
        }

        public void ChangePrice(CraftingResources resources, CraftingInventory inventory, EconomyWallet wallet, int change)
        {
            CheckInventory(inventory);
            var craftingInventory = _craftingRequests[inventory];
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
            if (_craftingRequests.ContainsKey(inventory))
            {
                if (_craftingRequests[inventory].ContainsKey(resource))
                {
                    var request = _craftingRequests[inventory][resource];
                    _requestWallets[request].EarnMoney(request.Reward);
                    _craftingRequests[inventory].Remove(resource);
                    _requestWallets.Remove(request);
            
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
            var validRequests = currentRequests.Count < maxRequests;
            if (containsResource && validRequests)
            {
                Debug.Log("Took request");
                craftingRequestRecord.AddRequest(requestTaker, takeRequest);
                _craftingRequests[inventory].Remove(takeRequest.Resource);
            }

            Refresh();
        }

        public float[] GetSenses(AdventurerAgent agent)
        {
            var craftingRequests = GetAllCraftingRequests();
            
            return CraftingResourceRequest.GetSenses(craftingRequests, 5);
        }

        public float[] GetSenses(ShopAgent agent)
        {
            var craftingRequests = GetAllCraftingRequests(agent.craftingInventory);
            return CraftingResourceRequest.GetSenses(craftingRequests, 5);
        }

        public void Update()
        {
            var currentTime = Time.time;
            var toRemove = new List<Tuple<CraftingResources, CraftingInventory>>();
            var removedRequests = false;
            foreach (var entry in _craftingRequests)
            {
                foreach (var item in entry.Value)
                {
                    if (Math.Abs(item.Value.CreationTime - currentTime) > 30)
                    {
                        var r = new Tuple<CraftingResources, CraftingInventory>(item.Value.Resource, entry.Key);
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
    }
}
