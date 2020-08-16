using System;
using UnityEngine;

namespace TurnBased.Scripts
{
	[Serializable]
	public class FighterData
	{
		public string unitName;
		public int maxHp;
		public int damage;

		public Sprite sprite;
		
		public int CurrentHp { get; private set; }

		public FighterData(Sprite sprite, string unitName, int maxHp, int currentHp, int damage)
		{
			this.sprite = sprite;
			this.unitName = unitName;
			this.maxHp = maxHp;
			this.damage = damage;
			CurrentHp = currentHp;
		}
		
		public FighterData(FighterData original)
		{
			sprite = original.sprite;
			unitName = original.unitName;
			maxHp = original.maxHp;
			damage = original.damage;
			CurrentHp = original.maxHp;
		}
		
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
	}
	
	[CreateAssetMenu]
	public class FighterUnit : ScriptableObject
	{
		public FighterData data;

		public FighterDropTable fighterDropTable;
		
		private void Init(FighterUnit fighterUnit)
		{
			data = new FighterData(fighterUnit.data);
			fighterDropTable = fighterUnit.fighterDropTable;
			fighterDropTable.ValidateTable();
		}

		public static FighterUnit GenerateItem(FighterUnit selectedItem)
		{
			var generatedItem = CreateInstance<FighterUnit>();
			generatedItem.Init(selectedItem);
			return generatedItem;
		}
	}
}
