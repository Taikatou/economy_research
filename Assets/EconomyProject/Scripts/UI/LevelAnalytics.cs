using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class LevelAnalytics : MonoBehaviour
    {
        public Text text;
        private void Update()
        {
            var txt = "";
            var agents = FindObjectsOfType<AdventurerAgent>();
            foreach (var agent in agents)
            {
                txt += agent.levelComponent.TotalExp + "\t" + agent.levelComponent.Level + "\n";
            }

            text.text = txt;
        }
    }
}
