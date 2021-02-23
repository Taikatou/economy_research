using UnityEngine;

namespace EconomyProject.Scripts.UI.Token
{
	public class AgentToken<T, TQ> : MonoBehaviour
	{
		public T agent;
		public TQ previousChoice;

		protected Transform agentList;
		protected Transform shop;
		protected Transform request;

		[Header("Adventurer only")]
		protected Transform arenaForest;
		protected Transform arenaMountain;
		protected Transform arenaSea;
		protected Transform arenaVolcano;

		[Header("Craftman only")]
		protected Transform craftPlace;

		public void Start()
		{
			InitializeTransforms();

			// Init the token to the default position 
			this.transform.position = agentList.position;
		}

		public void InitializeTransforms()
		{
			//Get the transform in the siblings
			agentList = transform.parent;

			foreach (Transform child in transform.parent.parent)
			{
				switch (child.name)
				{
					case "Shop":
						shop = child;
						break;
					case "Requests":
						request = child;
						break;
					case "Crafting":
						craftPlace = child;
						break;
					case "Environment":
						arenaForest = child.GetChild(0);
						arenaMountain = child.GetChild(1);
						arenaSea = child.GetChild(2);
						arenaVolcano = child.GetChild(3);
						break;
				}
			}
		}
	}
}
