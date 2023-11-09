using Data;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class RequestAdventurerSystemBehaviour : MonoBehaviour, ISetup, IGetSystem
    {
        public RequestAdventurerSystem system;

        public void Start()
        {
            system.Start();
        }

        public void Setup()
        {
            // system.setup();
        }

        public EconomySystem<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> GetSystem => system;
    }
}
