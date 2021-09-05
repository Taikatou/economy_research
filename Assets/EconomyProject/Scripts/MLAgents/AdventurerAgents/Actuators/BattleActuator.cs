using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using Unity.MLAgents.Actuators;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Actuators
{
    public class BattleActuator : IActuator
    {
        public ActionSpec ActionSpec { get; }

        public string Name => "BattleActuator";
        
        private AdventurerSystem AdventurerSystem => AdventurerInput.AdventurerSystem.system;

        private readonly AdventurerAgent _agent;
        
        public BattleActuator(AdventurerAgent agent)
        {
            _agent = agent;
            ActionSpec = ActionSpec.MakeDiscrete(3);
        }
        public void OnActionReceived(ActionBuffers actionBuffers)
        {
            var action = (EAdventurerAgentChoices) actionBuffers.DiscreteActions[0];

            switch (action)
            {
                case EAdventurerAgentChoices.BattleAttack:
                    AdventurerSystem.OnAttackButton(_agent);
                    break;
                case EAdventurerAgentChoices.BattleHeal:
                    AdventurerSystem.OnHealButton(_agent);
                    break;
                case EAdventurerAgentChoices.BattleFlee:
                    AdventurerSystem.OnFleeButton(_agent);
                    break;
            }
        }

        public void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            throw new System.NotImplementedException();
        }

        public void Heuristic(in ActionBuffers actionBuffersOut)
        {
            throw new System.NotImplementedException();
        }

        public void ResetData()
        {
            throw new System.NotImplementedException();
        }
    }
}
