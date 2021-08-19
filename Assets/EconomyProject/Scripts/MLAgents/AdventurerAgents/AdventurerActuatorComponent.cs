using EconomyProject.Scripts.MLAgents.AdventurerAgents.Actuators;
using Unity.MLAgents.Actuators;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class AdventurerActuatorComponent : ActuatorComponent
    {
        public AdventurerAgent agent;
        
        private ActionSpec _mActionSpec;
        public override ActionSpec ActionSpec => _mActionSpec;
        public override IActuator[] CreateActuators()
        {
            var actuators = new IActuator[] { new BattleActuator(agent), new AdventurerActuator(agent) , new MovementActuator(agent)};
            _mActionSpec = CombineActuatorActionSpecs(actuators);
            return actuators;
        }
        
        static ActionSpec CombineActuatorActionSpecs(IActuator[] actuators)
        {
            var specs = new ActionSpec[actuators.Length];
            for (var i = 0; i < actuators.Length; i++)
            {
                specs[i] = actuators[i].ActionSpec;
            }
            return ActionSpec.Combine(specs);
        }
    }
}
