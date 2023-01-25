using Data;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class AdventurerSystemBehaviour : MonoBehaviour, ISetup, IGetSystem
    {
        public AdventurerSystem system;

        public void Start()
        {
            system.Start();
        }

        public void Setup()
        {
            system.Setup();
        }

        public void Update()
        {
            system.Update();
        }

        public EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> GetSystem => system;
    }
}
