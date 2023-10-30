using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts.AI;
using Unity.MLAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class ConfirmAbilities <T> where T : Agent, IAdventurerAgent
    {
        private const int AbilitiesCount = 3;
        public readonly Dictionary<T, HashSet<EAttackOptions>> SelectedAttacks;
        public bool Confirm { get; set; }
        public ConfirmAbilities()
        {
            SelectedAttacks = new Dictionary<T, HashSet<EAttackOptions>>();
        }
        public void StartConfirm()
        {
            Confirm = true;
        }

        public void ConfirmAbility(T agent, EAttackOptions option)
        {
            if (!SelectedAttacks.ContainsKey(agent))
            {
                SelectedAttacks.Add(agent, new HashSet<EAttackOptions>());
            }
            
            var abilities = PlayerActionMap.GetAbilities(agent.AdventurerType, agent.LevelComponent.Level);
            if (abilities.Contains(option))
            {
                if (!SelectedAttacks[agent].Contains(option) && SelectedAttacks[agent].Count < AbilitiesCount)
                {
                    SelectedAttacks[agent].Add(option);
                }   
            }
        }

        public bool Complete(int partySize)
        {
            var counter = 0;
            foreach(var pair in SelectedAttacks)
            {
                var abilities = pair.Value;
                var possibleAbilities = PlayerActionMap.GetAbilities(pair.Key.AdventurerType, pair.Key.LevelComponent.Level);
                if (abilities.Count == AbilitiesCount || abilities.Count == possibleAbilities.Count)
                {
                    counter++;
                }
            }

            return counter == partySize;
        }

        public ObsData[] GetObservations(T agent)
        {
            var obs = new CategoricalObsData<EAttackOptions>[]
            {
                new CategoricalObsData<EAttackOptions>(), 
                new CategoricalObsData<EAttackOptions>(),
                new CategoricalObsData<EAttackOptions>()
            };
            if (SelectedAttacks.ContainsKey(agent))
            {
                var index = 0;
                foreach (var attack in SelectedAttacks[agent])
                {
                    obs[index] = new CategoricalObsData<EAttackOptions>(attack);
                }
            }

            return obs;
        }
        
        public static int SensorCount => SensorUtils<EAttackOptions>.Length * 3;
    }
}
