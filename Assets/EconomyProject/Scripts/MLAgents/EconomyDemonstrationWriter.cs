using System.IO;
using Data;
using Unity.MLAgents.Demonstrations;

namespace EconomyProject.Scripts.MLAgents
{
    public class EconomyDemonstrationWriter : DemonstrationWriter
    {
        private readonly bool _crafter;
        public bool ShouldRecord => _crafter == UISpec.CraftActive;
        
        public EconomyDemonstrationWriter(Stream stream, bool shouldRecord) : base(stream)
        {
            _crafter = shouldRecord;
        }
    }
}
