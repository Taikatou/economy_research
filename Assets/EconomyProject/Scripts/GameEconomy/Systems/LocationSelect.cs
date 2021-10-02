using System;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public abstract class AdvancedLocationSelect<T, G, F> : LocationSelect<T> where T : Agent where F : Enum
{
    public abstract G GetItem(T agent, F state);

    public abstract int GetCurrentLocation(T agent, F state);
}

public abstract class LocationSelect<T> : MonoBehaviour, IMoveMenu<T> where T : Agent
{
    protected abstract int GetLimit(T agent);
    
    protected Dictionary<T, int> currentLocation;
    public virtual void Start()
    {
        currentLocation = new Dictionary<T, int>();
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
            if (currentLocation[agent] >= limit)
            {
                currentLocation[agent] = limit;
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
