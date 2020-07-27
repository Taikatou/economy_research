using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Craftsman
{
    public class CraftsmanAgentSpawner : AgentSpawner
    {
        public ShopInput shopInput;
        public override GameObject Spawn(GameObject toSpawnPrefab)
        {
            var agent = base.Spawn(toSpawnPrefab);
            var craftsman = agent.GetComponent<ShopAgent>();
            craftsman.shopInput = shopInput;
            return agent;
        }
    }
}
