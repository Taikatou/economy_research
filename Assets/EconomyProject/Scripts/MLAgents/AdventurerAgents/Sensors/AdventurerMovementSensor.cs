using System;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public abstract class AdventurerMovementSensor : BaseEconomySensor
    {
        public static bool debugObs = true;
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
                var obs =  EconomySystem.GetObservations(_agent);
                var outputData = new float[obs.Length];
                var counter = 0;
                foreach (var o in obs)
                {
                    outputData[counter] = o.data;
                }
                _data = outputData;

                if (debugObs)
                {
                    var outputString = GetName();
                    foreach (var o in obs)
                    {
                        outputString += o.name + ": " + o.data + "\t";
                    }
                    Debug.Log(outputString);
                }
            }
            else
            {
                Array.Clear(_data, 0, _data.Length);
            }
        }
    }
}
