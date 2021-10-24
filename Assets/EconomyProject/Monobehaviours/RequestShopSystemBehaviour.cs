using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.Requests.ShopLocationMaps;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.UI;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class RequestShopSystemBehaviour : MonoBehaviour, ISetup
    {
        public GetCurrentRequestsLocation getCurrentRequestsLocation;
        
        public RequestShopSystem system;

        public ShopMakeCraftingRequestLocationMap shopMakeCraftingRequestLocationMap;

        public void Start()
        {
            system.MakeRequestGetLocation = shopMakeCraftingRequestLocationMap;
            system.ChangePriceGetLocation = getCurrentRequestsLocation;
            getCurrentRequestsLocation.requestSystem = system.requestSystem;
            system.Start();
        }

        public void Setup()
        {
            system.Setup();
        }
    }
}
