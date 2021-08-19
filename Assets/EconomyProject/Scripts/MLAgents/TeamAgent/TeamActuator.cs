using Unity.MLAgents.Actuators;

namespace EconomyProject.Scripts.MLAgents.TeamAgent
{
    public class TeamActuator : IActuator
    {
        public void OnActionReceived(ActionBuffers actionBuffers)
        {
            throw new System.NotImplementedException();
        }

        public void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            throw new System.NotImplementedException();
        }

        public void Heuristic(in ActionBuffers actionBuffersOut)
        {
            throw new System.NotImplementedException();
        }

        public ActionSpec ActionSpec { get; }
        public string Name => "TeamActuator";
        public void ResetData()
        {
            throw new System.NotImplementedException();
        }
    }
}
