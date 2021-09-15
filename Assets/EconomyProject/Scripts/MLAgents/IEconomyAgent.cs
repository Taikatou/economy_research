using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents
{
    public interface IEconomyAgent
    {
        public void SetAction(int action);
        public IEnumerable<EnabledInput> GetEnabledInput();
    }
}
