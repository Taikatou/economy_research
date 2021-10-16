using System.Collections.Generic;
using Unity.MLAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class PartySubSystem<T> where T : Agent
    {
        private readonly int _partySize;
        private readonly List<T> _pendingAgents;
        private readonly Dictionary<T, SimpleMultiAgentGroup> _agentParties;
        
        public PartySubSystem(int partySize)
        {
            _partySize = partySize;
            _pendingAgents = new List<T>();
            _agentParties = new Dictionary<T, SimpleMultiAgentGroup>();
        }

        public void AddAgent(T agent)
        {
            _pendingAgents.Add(agent);
            if (_pendingAgents.Count >= _partySize)
            {
                var agentGroup = new SimpleMultiAgentGroup();
                foreach (var a in _pendingAgents)
                {
                    agentGroup.RegisterAgent(a);
                    _agentParties.Add(a, agentGroup);
                }

                _pendingAgents.Clear();
            }
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
    }
}
