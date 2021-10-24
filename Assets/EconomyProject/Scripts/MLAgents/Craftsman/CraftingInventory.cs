using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Craftsman
{
    public class CraftingInventory : MonoBehaviour
    {
        private Dictionary<ECraftingResources, int> _numResources;
        
        public void Start()
        {
            _numResources = new Dictionary<ECraftingResources, int>();
        }

        public void ResetInventory()
        {
            if (_numResources == null)
            {
                _numResources = new Dictionary<ECraftingResources, int>();
            }
            else
            {
                _numResources.Clear();   
            }
        }

        public int GetResourceNumber()
        {
            var count = 0;
            foreach (var entry in _numResources)
            {
                count += entry.Value;
            }

            return count;
        }

        public bool HasResources(ECraftingResources eCraftingResource)
        {
            var toReturn = false;
            if (_numResources != null)
            {
                toReturn = _numResources.ContainsKey(eCraftingResource);   
            }
            return toReturn;
        }

        public int GetResourceNumber(ECraftingResources key)
        {
            if (_numResources != null)
            {
                if (_numResources.ContainsKey(key))
                {
                    return _numResources[key];
                }
            }
            return 0;
        }

        public void AddResource(ECraftingResources resource, int count)
        {
            var hasResource = _numResources.ContainsKey(resource);
            if (!hasResource)
            {
                _numResources[resource] = 0;
            }

            _numResources[resource] += count;
        }

        public bool HasResources(CraftingRequirements resource)
        {
            var found = true;
            for(var i = 0; i < resource.resourcesRequirements.Count && found; i++)
            {
                var item = resource.resourcesRequirements[i];
                found = _numResources.ContainsKey(item.type);
                if (found)
                {
                    found = _numResources[item.type] < item.number;
                }
            }

            if (found)
            {
                foreach (var item in resource.resourcesRequirements)
                {
                    _numResources[item.type] -= item.number;
                }
            }
            return found;
        }
    }
}
