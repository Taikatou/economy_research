using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inventory
{
	public class ItemData : MonoBehaviour
	{
		private static Dictionary<string, int> _defaultDurabilities = new Dictionary<string, int>
		{
			{ "Unarmed", 0 },
			{ "Beginner Sword", 10 },
			{ "Intermediate Sword", 12 },
			{ "Advanced Sword", 14 },
			{ "Epic Sword", 20 },
			{ "Master Sword", 22 },
			{ "Ultimate Sword", 25 }
		};

		private static Dictionary<string, int> _defaultDamages = new Dictionary<string, int>
		{
			{"Unarmed", 5 },
			{ "Beginner Sword", 7 },
			{ "Intermediate Sword", 9 },
			{ "Advanced Sword", 10 },
			{ "Epic Sword", 12 },
			{ "Master Sword", 12 },
			{ "Ultimate Sword", 15 }
		};

		public static Dictionary<string, int> baseDurabilities = new Dictionary<string, int>
		{
			{ "Unarmed", 0 },
			{ "Beginner Sword", 10 },
			{ "Intermediate Sword", 12 },
			{ "Advanced Sword", 14 },
			{ "Epic Sword", 20 },
			{ "Master Sword", 22 },
			{ "Ultimate Sword", 25 }
		};

		public static Dictionary<string, int> baseDamages = new Dictionary<string, int>
		{
			{"Unarmed", 5 },
			{ "Beginner Sword", 7 },
			{ "Intermediate Sword", 9 },
			{ "Advanced Sword", 10 },
			{ "Epic Sword", 12 },
			{ "Master Sword", 12 },
			{ "Ultimate Sword", 15 }
		};

		public static int GetDefaultDurability(string itemName)
		{
			if (!baseDurabilities.ContainsKey(itemName))
			{
				Debug.Log("Wrong ItemName : " + itemName);
				return 0;
			}
			return baseDurabilities[itemName];
		}

		public static int GetDefaultDamage(string itemName)
		{
			if (!baseDamages.ContainsKey(itemName))
			{
				Debug.Log("Wrong ItemName : " + itemName);
				return 0;
			}
			return baseDamages[itemName];
		}


		public static void SetDefaultDurability(string itemName, int newDurability)
		{
			baseDurabilities[itemName] = newDurability;
		}

		public static void SetDefaultDamage(string itemName, int newDmg)
		{
			baseDamages[itemName] = newDmg;
		}

		public static void SetDefaultDurabilities(Dictionary<string, int> newDurabilities)
		{
			baseDurabilities = newDurabilities;
		}

		public static void SetDefaultDamages(Dictionary<string, int> newDamages)
		{
			baseDamages = newDamages;
		}

		//Useful to reset
		public static Dictionary<string, int> GetDefaultDamages()
		{
			return _defaultDamages;
		}
		public static Dictionary<string, int> GetDefaultDurabilities()
		{
			return _defaultDurabilities;
		}
	}
}