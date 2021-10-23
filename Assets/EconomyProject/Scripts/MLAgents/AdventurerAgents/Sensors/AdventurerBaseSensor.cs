using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class AdventurerBaseSensor : BaseEconomySensor
    {
        private readonly AdventurerAgent _agent;

        public override string GetName() => "AdventurerSensor";
        
        protected override float[] Data { get; }

        private static readonly int SensorCount = 4;

        public AdventurerBaseSensor(AdventurerAgent agent)
        {
            _agent = agent;
            
            Data = new float [SensorCount];
            MObservationSpec = ObservationSpec.Vector(SensorCount);
            Data[3] = (int)_agent.adventurerType;
        }

        public override void Update()
        {
            var walletMoney = _agent.wallet ? _agent.wallet.Money : 0.0f;
            var screen = (int) _agent.ChosenScreen;
            // todo connect health data
            var health = 0;
            if (_agent.fighterData.playerData != null)
            {
                health = _agent.fighterData.playerData.CurrentHp;
            }

            Data[0] = walletMoney;
            Data[1] = screen;
            Data[2] = health;
        }
    }
}
