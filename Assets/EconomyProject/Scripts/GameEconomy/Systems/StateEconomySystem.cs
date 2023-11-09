using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public abstract class StateEconomySystem<TAgent, TScreen, TInput> : EconomySystem<TAgent, TScreen, TInput> where TAgent : AgentScreen where TScreen : Enum where TInput : Enum
    {

    }
}
