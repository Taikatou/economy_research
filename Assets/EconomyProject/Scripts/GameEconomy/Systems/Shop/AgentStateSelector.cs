using System;
using System.Collections.Generic;
using EconomyProject.Scripts.Interfaces;
using Unity.MLAgents;

public class AgentStateSelector<TAgent, EStates> : ISetup where TAgent : Agent where EStates : Enum
{
    private readonly Dictionary<TAgent, EStates> agentStates;

    private readonly EStates _defaultState;

    public AgentStateSelector(EStates defaultState)
    {
        agentStates = new Dictionary<TAgent, EStates>();
        _defaultState = defaultState;
    }

    public EStates GetState(TAgent agent)
    {
        if (!agentStates.ContainsKey(agent))
        {
            agentStates.Add(agent, _defaultState);
        }

        return agentStates[agent];
    }
    
    public void SetState(TAgent agent, EStates state)
    {
        if (agentStates.ContainsKey(agent))
        {
            agentStates[agent] = state;
        }
        else
        {
            agentStates.Add(agent, state);
        }
    }

    public void Setup()
    {
        agentStates?.Clear();
    }
}
