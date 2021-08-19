using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class AdventurerBaseSensor : BaseEconomySensor
    {
        private readonly AdventurerAgent _agent;

        public override string GetName() => "AdventurerSensor";
        
        protected override float[] Data { get; }

        public AdventurerBaseSensor(AdventurerAgent agent)
        {
            _agent = agent;
            Data = new float [2];
            MObservationSpec = ObservationSpec.Vector(2);
        }

        public override void Update()
        {
            var walletMoney = _agent.wallet ? _agent.wallet.Money : 0.0f;
            var screen = (float) _agent.ChosenScreen;

            Data[0] = walletMoney;
            Data[1] = screen;
        }
    }
}
