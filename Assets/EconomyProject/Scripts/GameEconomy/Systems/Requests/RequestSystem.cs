﻿using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using Unity.MLAgents.Sensors;
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

        public Dictionary<ECraftingResources, int> DefaultResourcePrices = new()
        {
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

		public static Dictionary<AgentType, int> StartMoney => new()
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

        private Dictionary<ECraftingResources, List<CraftingResourceRequest>> GetAllCraftingRequestsObservations()
        {
            var returnList = new Dictionary<ECraftingResources, List<CraftingResourceRequest>>
            {
                {ECraftingResources.Gem, new List<CraftingResourceRequest>()},
                {ECraftingResources.Metal, new List<CraftingResourceRequest>()},
                {ECraftingResources.Wood, new List<CraftingResourceRequest>()},
                {ECraftingResources.DragonScale, new List<CraftingResourceRequest>()},
            };
            
            foreach (var entry in _craftingRequests)
            {
                foreach (var entry2 in entry.Value)
                {
                    if (entry2.Value != null)
                    {
                        
                        returnList[entry2.Key].Add(entry2.Value);   
                    }
                }
            }

            return returnList;
        }

        public Dictionary<ECraftingResources, CraftingResourceRequest> GetAllCraftingRequestsObservations(CraftingInventory inventory)
        {
            var returnList = new Dictionary<ECraftingResources, CraftingResourceRequest>
            {
                {ECraftingResources.Gem, null},
                {ECraftingResources.Metal, null},
                {ECraftingResources.Wood, null},
                {ECraftingResources.DragonScale, null},
            };
            if (_craftingRequests.ContainsKey(inventory))
            {
                foreach (var entry2 in _craftingRequests[inventory])
                {
                    returnList[entry2.Key] = entry2.Value;   
                }
            }

            return returnList;
        }

        public Dictionary<ECraftingResources, CraftingResourceRequest> GetAllCraftingRequests(CraftingInventory inventory)
        {
            var returnList = new Dictionary<ECraftingResources, CraftingResourceRequest>();
            if (_craftingRequests.ContainsKey(inventory))
            {
                foreach (var entry2 in _craftingRequests[inventory])
                {
                    returnList.Add(entry2.Key, entry2.Value);
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
        
        public bool MakeRequest(ECraftingResources resources, CraftingInventory inventory, EconomyWallet wallet)
        {
            bool CheckExchange(CraftingResourceRequest request)
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
                    return true;
                }
                return false;
            }
            CheckInventory(inventory);

            var requestNumber = GetRequestNumber(inventory, resources);
            var canRequest = requestNumber < 10;
            var success = false;
			if (canRequest)
            {
                var containsKey = _craftingRequests[inventory].ContainsKey(resources);

				if (!containsKey)
                {
					var icon = GetIconByResource(resources);
                    var newResource = new CraftingResourceRequest(resources, inventory, GetDefaultPrice(resources), icon);
                    _requestWallets.Add(newResource, wallet);
                    success = CheckExchange(newResource);
                }
                else
                {
                    success = CheckExchange(_craftingRequests[inventory][resources]);
                }
            }

            Refresh();
            return success;
        }

        private int GetDefaultPrice(ECraftingResources resource)
        {
            var craftingRequests = GetAllCraftingRequestsObservations();
            if (craftingRequests.ContainsKey(resource))
            {
                var count = craftingRequests[resource];
                var prices = new List<int> {DefaultResourcePrices[resource]};
                foreach (var c in count)
                {
                    prices.Add(c.Price);
                }

                return (int) prices.Average();
            }
            else
            {
                return DefaultResourcePrices[resource];
            }
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
                            wallet.EarnMoney(Mathf.Abs(rewardDifference), false);
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
                        _requestWallets[request].EarnMoney(request.Reward, false);   
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
            
            var currentRequests = craftingRequestRecord.GetCurrentRequests(requestTaker);
            var validRequests = currentRequests.Length < maxRequests;
            if (containsResource && validRequests)
            {
                craftingRequestRecord.TakeRequest(requestTaker, takeRequest);
                _craftingRequests[inventory].Remove(takeRequest.Resource);
            }

            Refresh();
        }
        
        public void GetObservations(BufferSensorComponent bufferSensorComponent)
        {
            var craftingRequests = GetAllCraftingRequestsObservations();
            
            CraftingResourceRequest.GetObservations(craftingRequests, bufferSensorComponent);
        }
        
        public ObsData[] GetObservations(ShopAgent agent, BufferSensorComponent bufferSensorComponent)
        {
            var craftingRequests = GetAllCraftingRequestsObservations(agent.craftingInventory);
            CraftingResourceRequest.GetObservations(craftingRequests, bufferSensorComponent);
            return Array.Empty<ObsData>();
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
            if (SystemTraining.RemoveRequestTime)
            {
                UpdateRemove();
            }
        }
    }
}
