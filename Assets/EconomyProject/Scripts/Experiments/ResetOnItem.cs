using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.Interfaces;
using Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.Experiments
{
    public class ResetOnItem : MonoBehaviour
    {
        public static bool bSetupSystems;
        
        public UsableItem endItem;

        public bool resetOnComplete;

        public ShopCraftingSystemBehaviour shopCraftingBehaviour;

        private IEnumerable<AdventurerAgent> AdventurerAgents => FindObjectsOfType<AdventurerAgent>();
        private IEnumerable<ShopAgent> ShopAgents => FindObjectsOfType<ShopAgent>();

        public void Start()
        {
            shopCraftingBehaviour.system.shopSubSubSystem.onPurchaseItem = OnPurchase;
        }

        private void OnPurchase(UsableItem item, ShopAgent shop)
        {
            if (resetOnComplete)
            {
                if (item.itemDetails.itemName == endItem.itemDetails.itemName)
                {
                    ResetAgents();
                }
            }
        }

        public void ResetAgents()
        {
            foreach (var agent in AdventurerAgents)
            {
                agent.EndEpisode();
            }

            foreach (var agent in ShopAgents)
            {
                agent.EndEpisode();
            }
        }

        public void SetupSystems()
        {
            var setup = FindObjectsOfType<MonoBehaviour>().OfType<ISetup>();
            foreach (var s in setup)
            {
                s.Setup();
            }
        }

        public void Update()
        {
            if (bSetupSystems)
            {
                SetupSystems();
                bSetupSystems = false;
            }
        }
    }
}
