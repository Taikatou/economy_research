using UnityEngine;
using UnityEngine.UI;

namespace TurnBased
{
	public class BattleHud : MonoBehaviour
	{
		public Text nameText;
		public Text levelText;
		public Slider hpSlider;

		public void SetHud(FighterUnit fighterUnit)
		{
			nameText.text = fighterUnit.unitName;
			levelText.text = "Lvl " + fighterUnit.unitLevel;
			hpSlider.maxValue = fighterUnit.maxHp;
			hpSlider.value = fighterUnit.currentHp;
		}

		public void SetHp(int hp)
		{
			hpSlider.value = hp;
		}
	}
}
