using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerInventoryScrollView : AbstractScrollList<ItemUi, AdventurerInventoryScrollButton>
    {
        public GetCurrentAdventurerAgent adventurerAgent;

        protected override ILastUpdate LastUpdated => adventurerAgent.CurrentAgent.Inventory;

        protected override List<ItemUi> GetItemList()
        {
            var itemList = new List<ItemUi>();
            foreach (var item in adventurerAgent.CurrentAgent.Inventory.Items)
            {
                itemList.Add(new ItemUi
                {
                    ItemDetails = item.Value[0].itemDetails,
                    Number = item.Value.Count
                });
            }
            //Debug.Log("Inventory count : " + itemList.Count);
            return itemList;
        }
        public override void SelectItem(ItemUi item, int number = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
