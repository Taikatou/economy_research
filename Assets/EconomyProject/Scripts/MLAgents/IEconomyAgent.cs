using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems;

namespace EconomyProject.Scripts.MLAgents
{
    public interface IEconomyAgent
    {
        public void SetAction(int action, bool shop);
        public IEnumerable<EnabledInput> GetEnabledInput();

        public List<EnabledInput[]> GetEnabledInputNew();
        
        public int HalfSize { get; }
    }
}
