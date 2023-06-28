using System.Collections.Generic;
using EconomyProject.Scripts.Interfaces;
using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public abstract class LocationSelect<T> : MonoBehaviour, IMoveMenu<T>, ISetup where T : Agent
    {
        public abstract int GetLimit(T agent);

        protected Dictionary<T, int> CurrentLocation => _currentLocation ??= new Dictionary<T, int>();

        private Dictionary<T, int> _currentLocation;

        public virtual void Setup()
        {
            CurrentLocation.Clear();
        }

        public void RemoveAgent(T agent)
        {
            if (CurrentLocation.ContainsKey(agent))
            {
                CurrentLocation.Remove(agent);   
            }
        }
    
        public int GetCurrentLocation(T agent)
        {
            var toReturn = 0;
            if (agent != null)
            {
                if (!CurrentLocation.ContainsKey(agent))
                {
                    CurrentLocation.Add(agent, 0);
                }
                else
                {
                    var limit = GetLimit(agent);
                    if (CurrentLocation[agent] >= limit && CurrentLocation[agent] > 0)
                    {
                        CurrentLocation[agent] = limit - 1;
                    }
                }
                toReturn = CurrentLocation[agent];
            }

            return toReturn;
        }

        public float GetObs(T agent)
        {
            var limit = GetLimit(agent);
            return limit==0? 0 : GetCurrentLocation(agent) / GetLimit(agent);
        }

        protected virtual bool CircleOption => false;

        public void MovePosition(T agent, int movement)
        {
            var currentPosition = GetCurrentLocation(agent);
            var newPosition = currentPosition + movement;
            var limit = GetLimit(agent);

            if (newPosition >= limit && CircleOption)
            {
                CurrentLocation[agent] = 0;
            }
            else if (newPosition >= 0 && newPosition < limit)
            {
                CurrentLocation[agent] = newPosition;
            }
        }
    }
}
