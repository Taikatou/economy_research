using Data;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors
{
    public class AdventurerBaseSensor : BaseEconomySensor
    {
        private readonly AdventurerAgent _agent;

        public override string GetName() => "AdventurerSensor";

        protected override float[] Data => _data;

        private float[] _data;

        public AdventurerBaseSensor(AdventurerAgent agent)
        {
            _agent = agent;

            var obs = GetData();
            _data = new float [obs.Length];
            MObservationSpec = ObservationSpec.Vector(obs.Length);
        }

        private float[] GetData()
        {
            var walletMoney = _agent.wallet ? _agent.wallet.Money : 0.0f;
            // todo connect health data
            var health = _agent.fighterData.PlayerData?.GetObs ?? 0;
            var screen = _agent.ChosenScreen != null ? _agent.ChosenScreen : EAdventurerScreen.Main;  

            var obsData = new ObsData[]
            {
                new SingleObsData
                {
                    data = health,
                    Name = "health"
                },
                new CategoricalObsData<EAdventurerScreen>(screen)
                {
                    Name="Screen"
                },
                new SingleObsData
                {
                    data=walletMoney,
                    Name="Wallet"
                }
            };
            return ObsData.GetEnumerableData(obsData);
        }

        public override void Update()
        {
            var obs = GetData();
            _data = new float [obs.Length];
        }
    }
}
