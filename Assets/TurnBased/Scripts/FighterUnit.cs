using UnityEngine;

namespace TurnBased.Scripts
{
	[CreateAssetMenu]
	public class FighterUnit : ScriptableObject
	{
		public FighterData data;

		public FighterDropTable fighterDropTable;
		
		public void Awake()
		{
			fighterDropTable.ValidateTable();
		}
	}
}
