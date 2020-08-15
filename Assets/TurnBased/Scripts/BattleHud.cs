
using UnityEngine;
using UnityEngine.UI;

namespace TurnBased.Scripts
{
	public class BattleHud : MonoBehaviour
	{
		public Text nameText;
		public Text levelText;
		public Slider hpSlider;

		private FighterUnit _fighterUnit;

		public void SetHud(FighterUnit fighterUnit)
		{
			_fighterUnit = fighterUnit;
			nameText.text = fighterUnit.unitName;
			levelText.text = "Lvl ?";
			hpSlider.maxValue = fighterUnit.maxHp;
			hpSlider.value = fighterUnit.CurrentHp;
		}

		private void Update()
		{
			hpSlider.value = _fighterUnit.CurrentHp;
		}
	}
}
