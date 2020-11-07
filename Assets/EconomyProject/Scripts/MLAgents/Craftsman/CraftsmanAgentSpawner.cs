using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Craftsman
{
    public class CraftsmanAgentSpawner : AgentSpawner
    {
        public ShopInput shopInput;

        protected override GameObject Spawn(GameObject toSpawnPrefab)
        {
            var prefabGo = base.Spawn(toSpawnPrefab);
            var craftsman = prefabGo.GetComponent<ShopAgent>();
            craftsman.shopInput = shopInput;
            return prefabGo;
        }
    }
}
