using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class AdventurerRest : EconomySystem<BaseAdventurerAgent, EAdventurerScreen, EAdventurerScreen>
    {
        public override EAdventurerScreen ActionChoice => (EAdventurerScreen.Rest);
        public override bool CanMove(BaseAdventurerAgent agent)
        {
            return true;
        }

        public override ObsData[] GetObservations(BaseAdventurerAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            throw new System.NotImplementedException();
        }
    }
}
