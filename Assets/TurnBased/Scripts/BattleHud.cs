using System;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBased.Scripts
{
	public class BattleHud : MonoBehaviour
	{
		public Text nameText;
		public Text levelText;
		public Slider hpSlider;

		private GetCurrentHP _fighterUnit;

		public void SetHud<T, Q>(BaseFighterData<T, Q> fighterUnit) where T : Enum where Q : Enum
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
