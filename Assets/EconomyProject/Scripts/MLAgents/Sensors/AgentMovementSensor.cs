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

        protected bool CanViewConstant;

        protected abstract int SensorCount { get; }

        protected AgentMovementSensor(TAgent agent)
        {
            CanViewConstant = Observations.CanViewConstant;
            _agent = agent;
            
            _data = new float [SensorCount];
            MObservationSpec = ObservationSpec.Vector(SensorCount);
        }

        private float[] GetData()
        {
            var obs =  EconomySystem.GetObservations(_agent);
            /*var oneHot = new float [(int)EAdventurerScreen.Adventurer];
            var index = Convert.ToInt32(EconomySystem.ActionChoice);
            oneHot[index] = 1;*/
            
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

            if (Observations.DebugObs)
            {
                var outputString = GetName() + "\t" + outputData.Count + "\t" + _data.Length;
                Debug.Log(outputString);
            }

            return outputData.ToArray();
        }
        
        public override void Update()
        {
            var d = GetData();
            if (d.Length <= _data.Length)
            {
                Array.Clear(_data, 0, _data.Length);
                if (_agent.ChosenScreen.Equals(ValidScreen) || CanViewConstant)
                {
                    Array.Copy(d, _data, _data.Length);
                }
            }
        }
    }
}
