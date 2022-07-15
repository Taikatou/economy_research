using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Battle
{
    public class PlayerTurnSampleButton : SampleButton<FightingData>
    {
        public Text playerNameText;
        protected override void SetupButton()
        {
            playerNameText.text = ItemDetails.PlayerFighterData.UnitName;
        }

        protected override bool Selected()
        {
            return ItemDetails.BattleSubSystem.IsTurn(ItemDetails.PlayerFighterData);
        }
    }
}
