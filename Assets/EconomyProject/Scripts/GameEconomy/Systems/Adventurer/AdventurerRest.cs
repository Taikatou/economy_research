using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public class AdventurerRest : EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerScreen>
    {
        public override EAdventurerScreen ActionChoice => (EAdventurerScreen.Rest);
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override ObsData[] GetObservations(AdventurerAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            throw new System.NotImplementedException();
        }
    }
}
