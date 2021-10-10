using System;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public abstract class AgentMovementSensor<TAgent, TScreen, TChoice> : BaseEconomySensor where TAgent : AgentScreen<TScreen> where TScreen : Enum where TChoice : Enum
    {
        private float[] _data;
        private static bool debugObs = true;
        protected bool canViewConstant;
        private readonly TAgent _agent;
        protected abstract TScreen ValidScreen { get; }
        protected abstract EconomySystem<TAgent, TScreen, TChoice> EconomySystem { get; }
        protected override float[] Data => _data;
        protected abstract int SensorCount { get; }

        protected AgentMovementSensor(TAgent agent)
        {
            _agent = agent;
            
            _data = new float [SensorCount];
            MObservationSpec = ObservationSpec.Vector(SensorCount);
        }
        
        public override void Update()
        {
            if (_agent.ChosenScreen.Equals(ValidScreen) || canViewConstant)
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
                    var outputString = GetName() + "\t";
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
