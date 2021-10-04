using System;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public abstract class AdventurerMovementSensor : BaseEconomySensor
    {
        private readonly AdventurerAgent _agent;
        protected abstract EAdventurerScreen ValidScreen { get; }
        protected abstract EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> EconomySystem
        {
            get;
        }

        protected override float[] Data => _data;

        private float[] _data;

        protected abstract int SensorCount { get; }

        protected AdventurerMovementSensor(AdventurerAgent agent)
        {
            _agent = agent;
            
            _data = new float [SensorCount];
            MObservationSpec = ObservationSpec.Vector(SensorCount);
        }
        
        public override void Update()
        {
            if (_agent.ChosenScreen == ValidScreen)
            {
                _data = EconomySystem.GetObservations(_agent);
            }
            else
            {
                Array.Clear(_data, 0, _data.Length);
            }
        }
    }
}
