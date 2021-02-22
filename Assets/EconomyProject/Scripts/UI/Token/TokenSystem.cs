using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Token
{
	public class TokenSystem : MonoBehaviour
	{
		[Header("Adventurer")]
		public GetCurrentAdventurerAgent getAdventurerAgents;
		public GameObject adventurerToken;
		public Transform adventurerList;

		[Header("Craftman")]
		public GetCurrentShopAgent getCraftmanAgents;
		public GameObject craftmanToken;
		public Transform craftmanList;

		public bool mustInit = false;

		void Update()
		{
			if (getAdventurerAgents.GetAgents.Length > 0 && getCraftmanAgents.GetAgents.Length > 0 && mustInit == false)
			{
				mustInit = true;
				Init();
			}
		}

		/// <summary>
		/// Initialize the adventurers and craftmen tokens
		/// </summary>
		public void Init()
		{
			foreach (AdventurerAgent agent in getAdventurerAgents.GetAgents)
			{
				GameObject newAdventurerToken = adventurerToken;
				newAdventurerToken.GetComponent<AdventurerToken>().agent = agent;
				Instantiate(newAdventurerToken, adventurerList);
			}
			foreach (ShopAgent agent in getCraftmanAgents.GetAgents)
			{
				GameObject newCraftmanToken = craftmanToken;
				newCraftmanToken.GetComponent<CraftmanToken>().agent = agent;
				Instantiate(newCraftmanToken, craftmanList);
			}
		}

		/// <summary>
		/// Destroy all tokens
		/// </summary>
		public void ResetTokens()
		{
			foreach(Transform child in adventurerList)
			{
				Destroy(child);
			}
			foreach (Transform child in craftmanList)
			{
				Destroy(child);
			}
		}

	}
}


