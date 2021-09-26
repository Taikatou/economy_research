using System;
using System.Collections.Generic;
using Unity.MLAgents;

public class AgentStateSelector<TAgent, EStates> where TAgent : Agent where EStates : Enum
{
    public Dictionary<TAgent, EStates> agentStates;

    private EStates _defaultState;

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
}
