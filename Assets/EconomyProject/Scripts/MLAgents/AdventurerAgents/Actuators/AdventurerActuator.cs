using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using Unity.MLAgents.Actuators;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Actuators
{
    public class AdventurerActuator : IActuator
    {
        private readonly AdventurerAgent _agent;
        
        public ActionSpec ActionSpec { get; }

        public string Name => "AdventurerActuator";
        
        public AdventurerActuator(AdventurerAgent agent)
        {
            ActionSpec = ActionSpec.MakeDiscrete(3);
            _agent = agent;
        }
        public void OnActionReceived(ActionBuffers actionBuffers)
        {
            
            var action = (EAdventurerAgentChoices) actionBuffers.DiscreteActions[0];

            switch (action)
            {
                case EAdventurerAgentChoices.AForest:
                    StartBattle(BattleEnvironments.Forest);
                    break;
                case EAdventurerAgentChoices.ASea:
                    StartBattle(BattleEnvironments.Sea);
                    break;
                case EAdventurerAgentChoices.AMountain:
                    StartBattle(BattleEnvironments.Mountain);
                    break;
                case EAdventurerAgentChoices.AVolcano:
                    StartBattle(BattleEnvironments.Volcano);
                    break;
            }
        }
        
        private void StartBattle(BattleEnvironments battleEnvironments)
        {
            AdventurerInput.AdventurerSystem.system.StartBattle(_agent, battleEnvironments);
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
