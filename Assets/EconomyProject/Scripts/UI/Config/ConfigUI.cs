using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Config
{
	public enum TabConfig { Items, Resources, Agents, Craft }

	public class ConfigUI : MonoBehaviour
	{
		public GameObject tabItems;
		public GameObject tabResources;
		public GameObject tabAgents;
		public GameObject tabCraft;

		private void Start()
		{
			tabItems.GetComponent<ListConfigItems>().SetupList();
			tabResources.GetComponent<ListConfigResources>().SetupList();
			tabAgents.GetComponent<ListConfigAgents>().SetupList();
			tabCraft.GetComponent<ListConfigCraft>().SetupList();

			//Item tab active by default
			SwitchTab(0);
		}

		//Tabs
		public void SwitchTab(int tabNum)
		{
			switch ((TabConfig)tabNum)
			{
				case TabConfig.Items:
					tabItems.SetActive(true);
					tabResources.SetActive(false);
					tabAgents.SetActive(false);
					tabCraft.SetActive(false);
					break;
				case TabConfig.Resources:
					tabItems.SetActive(false);
					tabResources.SetActive(true);
					tabAgents.SetActive(false);
					tabCraft.SetActive(false);
					break;
				case TabConfig.Agents:
					tabItems.SetActive(false);
					tabResources.SetActive(false);
					tabAgents.SetActive(true);
					tabCraft.SetActive(false);
					break;
				case TabConfig.Craft:
					tabItems.SetActive(false);
					tabResources.SetActive(false);
					tabAgents.SetActive(false);
					tabCraft.SetActive(true);
					break;
				default:
					Debug.LogWarning("Wrong TabConfig : " + tabNum);
					break;
			}
		}

	}
}

