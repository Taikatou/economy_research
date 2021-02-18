using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Config
{
	public class ConfigCraft : AbstractConfigComponent<List<CraftingMap>, UsableItem>
	{
		public int wood = 0;
		public int metal = 0;
		public int gem = 0;
		public int dragonScale = 0;

		public Text woodNumber;
		public Text metalNumber;
		public Text gemNumber;
		public Text dragonScaleNumber;

		public InputField woodInputField;
		public InputField metalInputField;
		public InputField gemInputField;
		public InputField dragonScaleInputField;

		protected ListConfigCraft listConfigCraft;

		public override void SetupAbstractComponent()
		{
			abstractList = GameObject.FindObjectOfType<ListConfigCraft>();
			listConfigCraft = GameObject.FindObjectOfType<ListConfigCraft>();
		}

		public new void Setup(UsableItem newItem, int newNumber)
		{
			//base.Setup(newItem, newNumber); //not adequate
			item = newItem;
			nameLabel.text = newItem.ToString();

			SetupOthersParameters();
		}

		public void SetupOthersParameters()
		{
			wood = 0;
			metal = 0;
			gem = 0;
			dragonScale = 0;

			ListConfigCraft list = GameObject.FindObjectOfType<ListConfigCraft>();
			
			foreach (CraftingMap craftingMap in list.shopCraftingSystemBehaviour.system.craftingSubSubSystem.craftingRequirement)
			{
				if(craftingMap.resource.resultingItem == item)
				{
					image.sprite = craftingMap.resource.resultingItem.itemDetails.icon;
					SetupRequiredResources(craftingMap.resource.resourcesRequirements);
				}
			}

			woodNumber.text = wood.ToString();
			metalNumber.text = metal.ToString();
			gemNumber.text = gem.ToString();
			dragonScaleNumber.text = dragonScale.ToString();
		}

		public void SetupRequiredResources(List<ResourceRequirement> listResourceRequirement)
		{
			foreach (ResourceRequirement resourceRequired in listResourceRequirement)
			{
				switch (resourceRequired.type)
				{
					case CraftingResources.Wood:
						wood = resourceRequired.number;
						break;
					case CraftingResources.Metal:
						metal = resourceRequired.number;
						break;
					case CraftingResources.Gem:
						gem = resourceRequired.number;
						break;
					case CraftingResources.DragonScale:
						dragonScale = resourceRequired.number;
						break;
					default:
						Debug.Log("Wrong ResourceRequiremenent : " + resourceRequired.type);
						break;
				}
			}
		}

		/********************************************Wood**********************************/
		public void IncreaseWood()
		{
			wood++;
			SetWood();
		}
		public void DecreaseWood()
		{
			if (wood <= 0)
			{
				return;
			}

			wood--;
			SetWood();
		}
		public void SetWoodInputField()
		{
			if (woodInputField.text == null || woodInputField.text == "")
			{
				woodInputField.text = wood.ToString();
				return;
			}
			else
			{
				wood = int.Parse(woodInputField.text);
			}
			SetWood();
		}
		public void SetWood()
		{
			woodInputField.text = wood.ToString();
			SetItem();
		}

		/********************************************Metal**********************************/
		public void IncreaseMetal()
		{
			metal++;
			SetMetal();
		}
		public void DecreaseMetal()
		{
			if (metal <= 0)
			{
				return;
			}

			metal--;
			SetMetal();
		}
		public void SetMetalInputField()
		{
			if (metalInputField.text == null || metalInputField.text == "")
			{
				metalInputField.text = metal.ToString();
				return;
			}
			else
			{
				metal = int.Parse(metalInputField.text);
			}
			SetMetal();
		}
		public void SetMetal()
		{
			metalInputField.text = metal.ToString();
			SetItem();
		}

		/********************************************Gem**********************************/
		public void IncreaseGem()
		{
			gem++;
			SetGem();
		}
		public void DecreaseGem()
		{
			if (gem <= 0)
			{
				return;
			}

			gem--;
			SetGem();
		}
		public void SetGemInputField()
		{
			if (gemInputField.text == null || gemInputField.text == "")
			{
				gemInputField.text = gem.ToString();
				return;
			}
			else
			{
				gem = int.Parse(gemInputField.text);
			}
			SetGem();
		}
		public void SetGem()
		{
			gemInputField.text = gem.ToString();
			SetItem();
		}

		/********************************************DragonScale**********************************/
		public void IncreaseDragonScale()
		{
			dragonScale++;
			SetDragonScale();
		}
		public void DecreaseDragonScale()
		{
			if (dragonScale <= 0)
			{
				return;
			}

			dragonScale--;
			SetDragonScale();
		}
		public void SetDragonScaleInputField()
		{
			if (dragonScaleInputField.text == null || dragonScaleInputField.text == "")
			{
				dragonScaleInputField.text = dragonScale.ToString();
				return;
			}
			else
			{
				dragonScale = int.Parse(dragonScaleInputField.text);
			}
			SetDragonScale();
		}
		public void SetDragonScale()
		{
			dragonScaleInputField.text = dragonScale.ToString();
			SetItem();
		}

		/********************************************SetRequirements**********************************/

		/// <summary>
		/// Recreate a list of all the requirements. This list will replace the default one.
		/// </summary>
		public List<ResourceRequirement> GetListRequirement()
		{
			List<ResourceRequirement> newListRequirement = new List<ResourceRequirement>();

			if(wood > 0)
			{
				ResourceRequirement woodRequirement = new ResourceRequirement(CraftingResources.Wood, wood);
				newListRequirement.Add(woodRequirement);
			}
			if (metal > 0)
			{
				ResourceRequirement metalRequirement = new ResourceRequirement(CraftingResources.Metal, metal);
				newListRequirement.Add(metalRequirement);
			}
			if (gem > 0)
			{
				ResourceRequirement gemRequirement = new ResourceRequirement(CraftingResources.Gem, gem);
				newListRequirement.Add(gemRequirement);
			}
			if (dragonScale > 0)
			{
				ResourceRequirement dragonScaleRequirement = new ResourceRequirement(CraftingResources.DragonScale, dragonScale);
				newListRequirement.Add(dragonScaleRequirement);
			}
			return newListRequirement;
		}

		/// <summary>
		/// Set the requirements
		/// </summary>
		public void SetItem()
		{
			GameObject.FindObjectOfType<ListConfigCraft>().SetListRequirements(item, GetListRequirement());
		}
	}
}
