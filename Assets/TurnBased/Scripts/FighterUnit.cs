using UnityEngine;

namespace TurnBased.Scripts
{
	[CreateAssetMenu]
	public class FighterUnit : ScriptableObject
	{
		public string unitName;
		public int maxHp;
		public int damage;

		public Sprite sprite;
		public int CurrentHp { get; private set; }

		public bool TakeDamage(int dmg)
		{
			Debug.Log(CurrentHp + "\t" + dmg);
			CurrentHp -= dmg;

			return CurrentHp <= 0;
		}

		public void Heal(int amount)
		{
			CurrentHp += amount;
			if (CurrentHp > maxHp)
			{
				CurrentHp = maxHp;	
			}
		}

		private void Init(FighterUnit fighterUnit)
		{
			unitName = fighterUnit.unitName;
			maxHp = fighterUnit.maxHp;
			sprite = fighterUnit.sprite;
			damage = fighterUnit.damage;
			
			ResetHp();
		}

		private void ResetHp()
		{
			CurrentHp = maxHp;
		}

		public static FighterUnit GenerateItem(FighterUnit selectedItem)
		{
			var generatedItem = CreateInstance<FighterUnit>();
			generatedItem.Init(selectedItem);
			return generatedItem;
		}
	}
}
