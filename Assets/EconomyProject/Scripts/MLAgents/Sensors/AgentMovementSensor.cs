using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Sensors
{
    public abstract class AgentMovementSensor<TAgent, TScreen, TChoice> : BaseEconomySensor where TAgent : AgentScreen where TScreen : Enum where TChoice : Enum
    {
        private readonly float[] _data;
        
        private readonly TAgent _agent;
        protected abstract TScreen ValidScreen { get; }
        protected abstract EconomySystem<TAgent, TScreen, TChoice> EconomySystem { get; }
        protected override float[] Data => _data;

        protected bool CanViewConstant;

        protected abstract int SensorCount { get; }

        protected AgentMovementSensor(TAgent agent, BufferSensorComponent[] buffer) : base(buffer)
        {
            CanViewConstant = Observations.CanViewConstant;
            _agent = agent;
            
            _data = new float [SensorCount];
            MObservationSpec = ObservationSpec.Vector(SensorCount);
        }

        protected bool ObserveObservations = false;

        private float[] GetData(BufferSensorComponent[] buffer)
        {
            var obs =  EconomySystem.GetObservations(_agent, buffer);
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

            if (ObserveObservations)
            {
                var outputString = GetName() + "\t" + outputData.Count + "\t" + _data.Length + "\n";
                foreach (var ob in obs)
                {
                    outputString += ob.Name += "\t";
                }
                Debug.Log(outputString);
            }

            return outputData.ToArray();
        }
        
        public override void Update()
        {
            var d = GetData(BufferSensorComponent);
            if (d.Length <= _data.Length)
            {
                if (d.Length != _data.Length)
                {
                    Debug.Log(d.Length + "\t" + _data.Length + "\t" + GetName());
                }
                Array.Clear(_data, 0, _data.Length);
                if (_agent.ChosenScreenInt.Equals(ValidScreen) || CanViewConstant)
                {
                    Array.Copy(d, _data, d.Length);
                }
            }
            else
            {
                Debug.LogError(GetName() + " sensor has error " + d.Length + "\t" + _data.Length);
            }
        }
    }
}
