using UnityEngine;

namespace TurnBased.Scripts
{
	[CreateAssetMenu]
	public class FighterObject : ScriptableObject
	{
		public FighterData data;

		public FighterDropTable fighterDropTable;
		
		public void Awake()
		{
			fighterDropTable.ValidateTable();
		}

		private void Init(FighterObject fighter)
		{
			data = new FighterData(fighter.data);
			fighterDropTable = FighterDropTable.CloneData(fighter.fighterDropTable);
		}
		
		public static FighterObject GenerateItem(FighterObject selectedItem)
		{
			var generatedItem = CreateInstance<FighterObject>();
			generatedItem.Init(selectedItem);
			return generatedItem;
		}
	}
}
