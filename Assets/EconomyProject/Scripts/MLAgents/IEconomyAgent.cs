using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems;

namespace EconomyProject.Scripts.MLAgents
{
    public interface IEconomyAgent
    {
        public void SetAction(int action, bool shop);
        public int HalfSize { get; }
        
        public IEnumerable<EnabledInput> GetEnabledInput();
    }
}
