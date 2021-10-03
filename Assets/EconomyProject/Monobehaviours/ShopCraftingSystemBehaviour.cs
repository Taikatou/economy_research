using EconomyProject.Scripts;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class ShopCraftingSystemBehaviour : MonoBehaviour
    {
        public CraftLocationMap craftLocationMap;
        public CraftingRequestLocationMap craftingLocation;
        public ShopCraftingSystem system;
        public ShopLocationMap shopLocationMap;

		public void Start()
		{
			system.Start();
            craftingLocation.shopCraftingSystem = system;
            craftLocationMap.craftingSystem = system;
            shopLocationMap.shopSubSystem = system;
            system.CraftingLocationMap = craftingLocation;
            system.CraftLocationMap = craftLocationMap;
            system.ShopLocationMap = shopLocationMap;
        }
    }
}
