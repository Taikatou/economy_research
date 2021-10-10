using EconomyProject.Scripts.GameEconomy;
using Unity.MLAgents.Actuators;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Actuators
{
    public class MovementActuator : IActuator
    {
        public ActionSpec ActionSpec { get; }

        public string Name => "BattleActuator";
        
        private AdventurerInput AdventurerInput => _agent.adventurerInput;

        private readonly AdventurerAgent _agent;
        
        public MovementActuator(AdventurerAgent agent)
        {
            _agent = agent;
            ActionSpec = ActionSpec.MakeDiscrete(4);
        }
        public void OnActionReceived(ActionBuffers actionBuffers)
        {
            var action = (EAdventurerAgentChoices) actionBuffers.DiscreteActions[0];

            switch (action)
            {
/*                case EAdventurerAgentChoices.MainMenu:
                    AdventurerInput.ChangeScreen(_agent, EAdventurerScreen.Main);
                    break;*/
                case EAdventurerAgentChoices.FindRequest:
                    AdventurerInput.ChangeScreen(_agent, EAdventurerScreen.Request);
                    break;
                case EAdventurerAgentChoices.Shop:
                    AdventurerInput.ChangeScreen(_agent, EAdventurerScreen.Shop);
                    break;
                case EAdventurerAgentChoices.Adventure:
                    AdventurerInput.ChangeScreen(_agent, EAdventurerScreen.Adventurer);
                    break;
            }
        }

        public void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            
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
