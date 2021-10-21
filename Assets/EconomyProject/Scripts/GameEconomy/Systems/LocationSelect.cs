using System;
using System.Collections.Generic;
using EconomyProject.Scripts.Interfaces;
using Unity.MLAgents;
using UnityEngine;

public abstract class LocationSelect<T> : MonoBehaviour, IMoveMenu<T>, ISetup where T : Agent
{
    public abstract int GetLimit(T agent);

    protected Dictionary<T, int> currentLocation => _currentLocation ??= new Dictionary<T, int>();

    private Dictionary<T, int> _currentLocation;

    public virtual void Setup()
    {
        currentLocation.Clear();
    }

    public void RemoveAgent(T agent)
    {
        if (currentLocation.ContainsKey(agent))
        {
            currentLocation.Remove(agent);   
        }
    }
    
    public int GetCurrentLocation(T agent)
    {
        if (!currentLocation.ContainsKey(agent))
        {
            currentLocation.Add(agent, 0);
        }
        else
        {
            var limit = GetLimit(agent);
            if (currentLocation[agent] >= limit && currentLocation[agent] > 0)
            {
                currentLocation[agent] = limit - 1;
            }
        }

        return currentLocation[agent];
    }

    public void MovePosition(T agent, int movement)
    {
        var newPosition = GetCurrentLocation(agent) + movement;
        if (newPosition >= 0 && newPosition < GetLimit(agent))
        {
            currentLocation[agent] = newPosition;
        }
    }
}
