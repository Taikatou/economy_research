using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class ShopCraftingSystemBehaviour : MonoBehaviour
    {
        public ShopLocationMap shopLocationMap;
        public CraftingRequestLocationMap craftingLocation;
        public ShopCraftingSystem system;

		public void Start()
		{
			system.Start();
            craftingLocation.shopCraftingSystem = system;
            shopLocationMap.craftingSystem = system;
            system.CraftingLocationMap = craftingLocation;
            system.shopLocationMap = shopLocationMap;
        }

		public void FixedUpdate()
        {
            system.Update();
        }
    }
}
