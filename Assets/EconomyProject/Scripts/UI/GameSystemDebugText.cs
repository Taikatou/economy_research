using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class GameSystemDebugText : MonoBehaviour
    {
        public AdventurerSystemBehaviour adventurerSystemBehaviour;
        public AdventurerInput adventurerInput;
        public Text text;

        private void Update()
        {
            var txt = "";
            var agents = FindObjectsOfType<BaseAdventurerAgent>();
            foreach (var agent in agents)
            {
                var system = adventurerInput.GetScreen(agent, EAdventurerScreen.Main);
                var systemText = "";
                switch (system)
                {
                    case EAdventurerScreen.Adventurer:
                        var adventurerState = adventurerSystemBehaviour.system.GetAdventureStates(agent);
                        switch (adventurerState)
                        {
                            case EAdventureStates.OutOfBattle:
                                systemText = "OutOfBattle";
                                break;
                            case EAdventureStates.InQueue:
                                systemText = "InQueue";
                                break;
                            case EAdventureStates.InBattle:
                                systemText = "InBattle";
                                break;
                            case EAdventureStates.ConfirmBattle:
                                systemText = "ConfirmBattle";
                                break;
                            case EAdventureStates.ConfirmAbilities:
                                systemText = "ConfirmAbilities";
                                break;
                        }
                        
                        break;
                    case EAdventurerScreen.Main:
                        systemText = "Main";
                        break;
                    case EAdventurerScreen.Request:
                        systemText = "Request";
                        break;
                }
                txt += systemText + "\n";
            }

            text.text = txt;   
        }
    }
}
