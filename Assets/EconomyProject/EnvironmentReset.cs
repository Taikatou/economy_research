using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

public class EnvironmentReset : MonoBehaviour
{
    public GameObject adventurerAgent;
    public GameObject shopAgent;
    public void ResetScript()
    {
        var agents = adventurerAgent.GetComponentsInChildren<AdventurerAgent>();
        foreach (var agent in agents)
        {
            agent.ResetEconomyAgent();
        }
        
        var shopAgents = shopAgent.GetComponentsInChildren<ShopAgent>();
        foreach (var agent in shopAgents)
        {
            agent.ResetEconomyAgent();
        }
    }
}
