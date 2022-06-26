
using UnityEngine;
using UnityEngine.UI;

namespace TurnBased.Scripts
{
	public class BattleHud : MonoBehaviour
	{
		public Text nameText;
		public Text levelText;
		public Slider hpSlider;

		private BaseFighterData _fighterUnit;

		public void SetHud(BaseFighterData fighterUnit)
		{
			_fighterUnit = fighterUnit;
			nameText.text = fighterUnit.UnitName;
			levelText.text = "Lvl " + fighterUnit.Level;
			hpSlider.maxValue = fighterUnit.MaxHp;
			hpSlider.value = fighterUnit.CurrentHp;
		}

		private void Update()
		{
			if (_fighterUnit != null)
			{
				hpSlider.value = _fighterUnit.CurrentHp;	
			}
		}
	}
}
