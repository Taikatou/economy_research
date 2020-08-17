using UnityEngine;

namespace TurnBased.Scripts
{
	[CreateAssetMenu]
	public class FighterObject : ScriptableObject
	{
		public FighterData data;

		public FighterDropTable fighterDropTable;

		private void Init(FighterObject fighter)
		{
			data = FighterData.Clone(fighter.data);
			fighterDropTable = fighter.fighterDropTable;
			fighterDropTable.ValidateTable();
		}
		
		public static FighterObject GenerateItem(FighterObject selectedItem)
		{
			var generatedItem = CreateInstance<FighterObject>();
			generatedItem.Init(selectedItem);
			return generatedItem;
		}
	}
}
