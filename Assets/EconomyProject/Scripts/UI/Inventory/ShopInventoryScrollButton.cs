using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class ShopInventoryScrollButton : SampleButton<ShopItem>
	{
        public Text nameLabel;
        public Text priceText;
        public Text stockText;

        public GameObject increasePriceBtn;
		public GameObject decreasePriceBtn;

		public int currentIndex;
		public bool selected;

		public Image iconImage;

		public void UpdateData(int CurrentIndex, bool Selected)
		{
			currentIndex = CurrentIndex;
			selected = Selected;
		}
		
		protected override void SetupButton()
        {

			nameLabel.text = ItemDetails.Item.itemDetails.itemName;
            priceText.text = ItemDetails.Price.ToString();
            stockText.text = ItemDetails.Number.ToString();
            iconImage.sprite = ItemDetails.Item.itemDetails.icon;

			//Can modify the price only when the item is in the shop && if the current agent is a shopAgent
			ShopScrollView parentScrollList = this.GetComponentInParent<ShopScrollView>();
			if (parentScrollList != null && parentScrollList.shopAgent != null)
			{
				increasePriceBtn.SetActive(true);
				decreasePriceBtn.SetActive(true);
			}
			else
			{
				increasePriceBtn.SetActive(false);
				decreasePriceBtn.SetActive(false);
			}
        }

		/// <summary>
		/// Increment price by +1
		/// </summary>
		public void IncreasePrice()
		{
			FindObjectOfType<ShopScrollView>().IncreasePrice(ItemDetails);
			UpdateButtonsPrice(ItemDetails.Price+1);
		}

		/// <summary>
		/// Increment price by -1
		/// Minimum price = 1
		/// </summary>
		public void DecreasePrice()
		{
			if(ItemDetails.Price - 1 <= 0)
			{
				return;
			}

			FindObjectOfType<ShopScrollView>().DecreasePrice(ItemDetails);
			UpdateButtonsPrice(ItemDetails.Price-1);
		}

		/// <summary>
		/// To refresh data of all the shop buttons
		/// It is useful to update the price text if the same type of sword is present in both inventory column and shop column and if the price is changed
		/// </summary>
		/// <param name="newPrice">New price to update</param>
		public void UpdateButtonsPrice(int newPrice)
		{
			foreach (ShopInventoryScrollButton btn in FindObjectsOfType<ShopInventoryScrollButton>())
			{
				if(ItemDetails.Item.itemDetails.itemName == btn.ItemDetails.Item.itemDetails.itemName)
				{
					btn.ItemDetails.Price = newPrice;
					btn.priceText.text = ItemDetails.Price.ToString();
				}
			}
		}

		protected override bool Selected()
		{
			var toReturn = false;
			if(selected)
			{
				toReturn = currentIndex == ItemDetails.Index;
			}

			return toReturn;
		}
	}
}
