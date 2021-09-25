using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class ShopCraftingSystemBehaviour : MonoBehaviour
    {
        public ShopCraftingSystem system;

		public void Start()
		{
			system.Start();
		}

		public void FixedUpdate()
        {
            system.Update();
        }
    }
}
