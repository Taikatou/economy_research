using System.IO;
using Unity.MLAgents.Demonstrations;

namespace EconomyProject.Scripts.MLAgents
{
    public class EconomyDemonstrationRecorder : DemonstrationRecorder
    {
        public bool crafting;

        protected DemonstrationWriter CreateDemonstrator(Stream stream)
        {
            return new EconomyDemonstrationWriter(stream, crafting);
        }
    }
}
