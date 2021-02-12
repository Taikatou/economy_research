using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
    public class ResetScript : MonoBehaviour
    {
        public DataLogger dataLogger;
        public GameObject agentParents;
        public void Reset()
        {
			/*
            var agents = agentParents.GetComponentsInChildren<AdventurerAgent>();
            foreach (var agent in agents)
            {
                agent.ResetEconomyAgent();
            }

            var dLogger = dataLogger.GetComponentInChildren<DataLogger>();
            dLogger.Reset();
			*/
        }
    }
}
