using System.Collections.Generic;
using Unity.MLAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class PartySubSystem<T> where T : Agent
    {
        private readonly int _partySize;
        protected readonly List<T> _pendingAgents;
        private Dictionary<T, SimpleMultiAgentGroup> _agentParties;

        protected PartySubSystem(int partySize)
        {
            _partySize = partySize;
            _pendingAgents = new List<T>();
            _agentParties = new Dictionary<T, SimpleMultiAgentGroup>();
        }

        public void RemoveAgent(T a)
        {
            if (_agentParties.ContainsKey(a))
            {
                _agentParties[a].UnregisterAgent(a);
                _agentParties.Remove(a);
            }
        }

        public void AddAgent(T agent)
        {
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

        public void AddReward(float reward, T agent)
        {
            if (_agentParties.ContainsKey(agent))
            {
                _agentParties[agent].AddGroupReward(reward);
            }
        }

        public void Setup()
        {
            _pendingAgents.Clear();
            _agentParties.Clear();
        }
    }
}
