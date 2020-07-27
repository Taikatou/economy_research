using UnityEngine;

namespace EconomyProject.Scripts.MLAgents
{
    public class AgentID : MonoBehaviour
    {
        public static int agentCounter = 0;

        [HideInInspector] public int agentId;

        private void Start()
        {
            agentId = agentCounter;
            agentCounter++;
        }
    }
}
