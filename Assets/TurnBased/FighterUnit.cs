using UnityEngine;

namespace TurnBased
{
	public class FighterUnit : MonoBehaviour
	{
		public string unitName;
		public int unitLevel;

		public int damage;

		public int maxHp;
		public int currentHp;

		public bool TakeDamage(int dmg)
		{
			currentHp -= dmg;

			return currentHp <= 0;
		}

		public void Heal(int amount)
		{
			currentHp += amount;
			if (currentHp > maxHp)
			{
				currentHp = maxHp;	
			}
		}

	}
}
