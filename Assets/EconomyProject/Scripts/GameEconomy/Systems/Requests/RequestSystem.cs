using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public class ResourceRequest
    {
        private bool _resourceAdded;
        public CraftingResources Resource { get; }
        public CraftingInventory Inventory { get; }
        public int Number { get; set; }
        public int Price { get; set; }

        private static readonly Dictionary<CraftingResources, int> _defaultPrice = new Dictionary<CraftingResources, int> {
            {CraftingResources.Wood, 5},
            {CraftingResources.Metal, 6},
            {CraftingResources.Gem, 7},
            {CraftingResources.DragonScale, 8}
        };

        public ResourceRequest(CraftingResources resource, CraftingInventory inventory, int number = 1)
        {
            Resource = resource;
            Inventory = inventory;
            Number = number;
            Price = _defaultPrice[resource];
        }

        public void TransferResource()
        {
            if (!_resourceAdded)
            {
                _resourceAdded = true;
                Inventory.AddResource(Resource, Number);
            }
        }
    }

    public class RequestSystem : LastUpdate
    {
        public RequestRecord requestRecord;

        private Dictionary<CraftingInventory, Dictionary<CraftingResources, ResourceRequest>> _craftingNumber;

        public Dictionary<CraftingResources, ResourceRequest> GetCraftingRequests(CraftingInventory inventory)
        {
            if (_craftingNumber.ContainsKey(inventory))
            {
                return _craftingNumber[inventory];
            }

            return new Dictionary<CraftingResources, ResourceRequest>();
        }

        public List<ResourceRequest> GetAllCraftingRequests()
        {
            var returnList = new List<ResourceRequest>();
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
            _craftingNumber = new Dictionary<CraftingInventory, Dictionary<CraftingResources, ResourceRequest>>();
            Refresh();
        }

        public bool MakeRequest(CraftingResources resources, CraftingInventory inventory)
        {
            CheckInventory(inventory);

            var requestNumber = GetRequestNumber(inventory, resources);
            var canRequest = requestNumber < 5;
            if (canRequest)
            {
                var containsKey = _craftingNumber[inventory].ContainsKey(resources);
                if (!containsKey)
                {
                    _craftingNumber[inventory].Add(resources, new ResourceRequest(resources, inventory));
                }
                else
                {
                    _craftingNumber[inventory][resources].Number++;
                }
            }

            Refresh();
            return canRequest;
        }

        private void CheckInventory(CraftingInventory inventory)
        {
            if (!_craftingNumber.ContainsKey(inventory))
            {
                _craftingNumber.Add(inventory, new Dictionary<CraftingResources, ResourceRequest>());
            }
        }

        public void IncreasePrice(CraftingResources resources, CraftingInventory inventory)
        {
            ChangePrice(resources, inventory, 1);
        }
        
        public void DecreasePrice(CraftingResources resources, CraftingInventory inventory)
        {
            ChangePrice(resources, inventory, -1);
        }

        private void ChangePrice(CraftingResources resources, CraftingInventory inventory, int number)
        {
            CheckInventory(inventory);
            var containsKey = _craftingNumber[inventory].ContainsKey(resources);
            if (containsKey)
            {
                if(_craftingNumber[inventory].ContainsKey(resources))
                {
                    _craftingNumber[inventory][resources].Price += number;
                    Debug.Log("price change");
                }
            }
            Refresh();
        }

        public void RemoveRequest(CraftingResources resource, CraftingInventory inventory)
        {
            _craftingNumber[inventory].Remove(resource);

            Refresh();
        }

        public bool TakeRequest(RequestTaker requestTaker, ResourceRequest takeRequest)
        {
            if (_craftingNumber[takeRequest.Inventory].ContainsKey(takeRequest.Resource))
            {
                requestRecord.AddRequest(requestTaker, takeRequest);
                _craftingNumber[takeRequest.Inventory].Remove(takeRequest.Resource);
            }

            Refresh();
            return true;
        }

        public void CompleteRequest(RequestTaker taker, ResourceRequest request)
        {
            requestRecord.CompleteRequest(taker, request);
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
