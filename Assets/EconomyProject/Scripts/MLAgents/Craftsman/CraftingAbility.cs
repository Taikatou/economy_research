using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Craftsman
{
    [System.Serializable]
    public struct CraftingMap
    {
        public CraftingChoice choice;
        public CraftingRequirements resource;
    }

    public enum CraftingChoice { Abstain, BeginnerSword, IntermediateSword, AdvancedSword, EpicSword, UltimateSwordOfPower}

    public class CraftingAbility : MonoBehaviour
    {
        public float UpdateTime { get; private set; }

        public List<CraftingMap> craftingRequirement;

        public bool Crafting { get; private set; }

        public float TimeToCreation { get; private set; }

        public InventoryItem ChosenCrafting { get; set; }

        public CraftingInventory CraftingInventory => GetComponent<CraftingInventory>();

        public AgentInventory Inventory => GetComponent<AgentInventory>();

        public float Progress
        {
            get
            {
                if (Crafting)
                {
                    return UpdateTime / TimeToCreation;
                }
                return 0;
            }
        }

        public void SetCraftingItem(CraftingChoice choice)
        {
            var foundChoice = craftingRequirement.Single(c => c.choice == choice);
            if (!Crafting && foundChoice.resource)
            {
                var hasResources = CraftingInventory.HasResources(foundChoice.resource);
                if (hasResources)
                {
                    StartCrafting(foundChoice);
                }
            }
        }

        private void StartCrafting(CraftingMap foundChoice)
        {
            UpdateTime = 0;
            Crafting = true;
            ChosenCrafting = foundChoice.resource.resultingItem;
            TimeToCreation = foundChoice.resource.timeToCreation;
        }

        public void SetCraftingItem(int choice)
        {
            SetCraftingItem((CraftingChoice) choice);
        }

        public void FinishCrafting()
        {
            var generatedItem = InventoryItem.GenerateItem(ChosenCrafting);
            Inventory.AddItem(generatedItem);
            Crafting = false;
        }

        private void Update()
        {
            if (Crafting)
            {
                UpdateTime += Time.deltaTime;
                if (UpdateTime >= TimeToCreation)
                {
                    FinishCrafting();
                }
            }
        }
    }
}
