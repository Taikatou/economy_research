using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts.AI;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class ConfirmAbilities
    {
        private const int abilitiesCount = 3;
        private readonly Dictionary<AdventurerAgent, HashSet<EAttackOptions>> _selectedAttacks;
        public bool Confirm { get; set; }
        public ConfirmAbilities()
        {
            _selectedAttacks = new Dictionary<AdventurerAgent, HashSet<EAttackOptions>>();
        }
        public void StartConfirm()
        {
            Confirm = true;
        }

        public void ConfirmAbility(AdventurerAgent agent, EAttackOptions option)
        {
            if (!_selectedAttacks.ContainsKey(agent))
            {
                _selectedAttacks.Add(agent, new HashSet<EAttackOptions>());
            }

            if (!_selectedAttacks[agent].Contains(option) && _selectedAttacks[agent].Count < abilitiesCount)
            {
                _selectedAttacks[agent].Add(option);
            }
        }

        public bool Complete(int partySize)
        {
            var counter = 0;
            foreach(var pair in _selectedAttacks)
            {
                var abilities = pair.Value;
                var possibleAbilities = PlayerActionMap.GetAbilities(pair.Key.AdventurerType, pair.Key.levelComponent.Level);
                if (abilities.Count == abilitiesCount || abilities.Count == possibleAbilities.Count)
                {
                    counter++;
                }
            }

            return counter == partySize;
        }

        public ObsData[] GetObservations(AdventurerAgent agent)
        {
            var obs = new CategoricalObsData<EAttackOptions>[]
            {
                new CategoricalObsData<EAttackOptions>(), 
                new CategoricalObsData<EAttackOptions>(),
                new CategoricalObsData<EAttackOptions>()
            };
            if (_selectedAttacks.ContainsKey(agent))
            {
                var index = 0;
                foreach (var attack in _selectedAttacks[agent])
                {
                    obs[index] = new CategoricalObsData<EAttackOptions>(attack);
                }
            }

            return obs;
        }
        
        public static int SensorCount => SensorUtils<EAttackOptions>.Length * 3;
    }
}
