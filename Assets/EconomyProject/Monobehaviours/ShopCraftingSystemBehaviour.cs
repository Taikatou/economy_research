using System;
using EconomyProject.Scripts;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.Interfaces;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class ShopCraftingSystemBehaviour : MonoBehaviour, ISetup
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
            system.SubmitToShopLocationMap = craftLocationMap;
            system.ShopLocationMap = shopLocationMap;
        }

        public void Setup()
        {
            system.Setup();
        }

        public void FixedUpdate()
        {
            system.FixedUpdate();
        }
    }
}
