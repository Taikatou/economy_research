using System.Collections.Generic;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerInventoryScrollView : AbstractScrollList<InventoryItem, AdventurerInventoryScrollButton>
    {
        public GetCurrentAdventurerAgent adventurerAgent;
        protected override LastUpdate LastUpdated => adventurerAgent.CurrentAgent.inventory;
        protected override List<InventoryItem> GetItemList()
        {
            var itemList = new List<InventoryItem>();
            foreach (var item in adventurerAgent.CurrentAgent.inventory.Items)
            {
                itemList.Add(item.Value);
            }
            Debug.Log(itemList.Count);
            return itemList;
        }
        public override void SelectItem(InventoryItem item, int number = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
