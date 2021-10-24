using System;
using System.Collections.Generic;
using Data;
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
        
        private readonly TAgent _agent;
        protected abstract TScreen ValidScreen { get; }
        protected abstract EconomySystem<TAgent, TScreen, TChoice> EconomySystem { get; }
        protected override float[] Data => _data;

        protected bool canViewConstant;
        
        public abstract int SensorCount { get; }

        protected AgentMovementSensor(TAgent agent)
        {
            canViewConstant = Observations.canViewConstant;
            _agent = agent;
            
            _data = new float [SensorCount];
            MObservationSpec = ObservationSpec.Vector(SensorCount);
        }

        public float[] GetData()
        {
            var obs =  EconomySystem.GetObservations(_agent);
            var outputData = new List<float>();
            foreach (var ob in obs)
            {
                try
                {
                    outputData.AddRange(ob.GetData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            if (Observations.debugObs)
            {
                var outputString = GetName() + "\t" + outputData.Count + "\t" + _data.Length;
                Debug.Log(outputString);
            }

            return outputData.ToArray();
        }
        
        public override void Update()
        {
            var d = GetData();
            if (_agent.ChosenScreen.Equals(ValidScreen) || canViewConstant)
            {
                Array.Copy(_data, d, _data.Length);
                _data = d;
            }
            else
            {
                Array.Clear(_data, 0, _data.Length);
            }
        }
    }
}
