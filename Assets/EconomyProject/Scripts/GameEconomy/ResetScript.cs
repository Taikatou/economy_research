using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
    public class ResetScript : MonoBehaviour
    {
        public void Reset()
        {
            var agents = GetComponentsInChildren<AdventurerAgent>();
            foreach (var agent in agents)
            {
                agent.ResetEconomyAgent();
            }

            var dLogger = GetComponentInChildren<DataLogger>();
            dLogger?.Reset();
        }
    }
}
