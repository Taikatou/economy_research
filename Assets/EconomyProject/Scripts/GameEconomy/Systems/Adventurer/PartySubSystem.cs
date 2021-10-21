using System;
using System.Collections.Generic;
using Unity.MLAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class PartySubSystem<T> where T : Agent
    {
        private readonly int _partySize;
        protected readonly List<T> _pendingAgents;
        private readonly Dictionary<T, SimpleMultiAgentGroup> _agentParties;

        protected PartySubSystem(int partySize)
        {
            _partySize = partySize;
            _pendingAgents = new List<T>();
            _agentParties = new Dictionary<T, SimpleMultiAgentGroup>();
        }

        public bool RemoveAgent(T a)
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

            return contains;
        }

        public void AddAgent(T agent)
        {
            if (_agentParties.ContainsKey(agent))
            {
                _agentParties[agent].UnregisterAgent(agent);
                _agentParties.Remove(agent);
            }
            _pendingAgents.Add(agent);
            if (_pendingAgents.Count >= _partySize)
            {
                var agentGroup = new SimpleMultiAgentGroup();
                CompleteParty(agentGroup);
            }
        }

        public virtual void CompleteParty(SimpleMultiAgentGroup agentGroup)
        {
            foreach (var a in _pendingAgents)
            {
                agentGroup.RegisterAgent(a);
                _agentParties.Add(a, agentGroup);
            }
            _pendingAgents.Clear();
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
            _pendingAgents.Clear();
            _agentParties.Clear();
        }
    }
}
