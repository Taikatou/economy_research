using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerInventoryScrollView : InventoryScrollView
    {
        // Update is called once per frame
        public override void SelectItem(ShopItem item, int number = 1)
        {
            throw new System.NotImplementedException();
        }

        protected override ShopAgent CurrentShop { get; }
    }
}
