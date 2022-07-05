using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public delegate void OnAddPlayer<in T>(T agent);
    public class PartySubSystem<T> where T : AdventurerAgent
    {
        private readonly int _partySize;
        public List<T> PendingAgents { get; private set; }
        private readonly Dictionary<T, SimpleMultiAgentGroup> _agentParties;

        private OnAddPlayer<T> OnAddPlayer;

        protected PartySubSystem(int partySize)
        {
            _partySize = partySize;
            PendingAgents = new List<T>();
            _agentParties = new Dictionary<T, SimpleMultiAgentGroup>();
        }

        public void RemoveAgent(T a)
        {
            var contains = _agentParties.ContainsKey(a);
            if (contains)
            {
                _agentParties[a].UnregisterAgent(a);
                _agentParties.Remove(a);
            }
            else
            {
                throw new Exception("DFSFASF");
            }
        }

        public void RemoveFromQueue(T agent)
        {
            PendingAgents.Remove(agent);
        }

        public void AddAgent(T agent)
        {
            if (_agentParties.ContainsKey(agent))
            {
                _agentParties[agent].UnregisterAgent(agent);
                _agentParties.Remove(agent);
            }
            PendingAgents.Add(agent);
            if (Full)
            {
                var agentGroup = new SimpleMultiAgentGroup();
                CompleteParty(agentGroup);
            }

            OnAddPlayer?.Invoke(agent);
        }

        public bool Full => PendingAgents.Count >= _partySize;

        public virtual void CompleteParty(SimpleMultiAgentGroup agentGroup)
        {
            foreach (var a in PendingAgents)
            {
                agentGroup.RegisterAgent(a);
                _agentParties.Add(a, agentGroup);
            }
            PendingAgents.Clear();
        }

        public void FinishBattle(IEnumerable<T> battleAgents)
        {
            foreach (var agent in battleAgents)
            {
                if (_agentParties.ContainsKey(agent))
                {
                    _agentParties[agent].UnregisterAgent(agent);
                    _agentParties.Remove(agent);
                }
            }
        }

        public void Setup()
        {
            foreach (var agent in _agentParties)
            {
                agent.Value.UnregisterAgent(agent.Key);
            }
            PendingAgents.Clear();
            _agentParties.Clear();
        }

        private static float MaxLevel = 10;

        public List<ObsData> GetObservations()
        {
            var obs = new List<ObsData>();
            var i = 0;
            foreach (var agent in PendingAgents)
            {
                obs.Add(new CategoricalObsData<EAdventurerTypes>(agent.AdventurerType));
                obs.Add(new SingleObsData
                {
                    data = (float)agent.levelComponent.Level / MaxLevel
                });
                i++;
            }
            for (; i < SystemTraining.PartySize - 1; i++)
            {
                obs.Add(new CategoricalObsData<EAdventurerTypes>(EAdventurerTypes.All));
                obs.Add(new SingleObsData
                {
                    data = 0
                });
            }
            return obs;
        }
    }
}
