using Data;
using Unity.MLAgents;

namespace EconomyProject.Scripts.MLAgents
{
    public class EconomyDecisionRequester : DecisionRequester
    {
        public bool crafter;

        protected override void MakeRequests(int academyStepCount)
        {
            if (crafter == UISpec.CraftActive || Academy.Instance.IsCommunicatorOn)
            {
                base.MakeRequests(academyStepCount);
            }
        }
    }
}
