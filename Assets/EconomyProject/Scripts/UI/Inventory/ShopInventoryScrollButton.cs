using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class ShopInventoryScrollButton : SampleButton<ShopItemUi>
	{
        public Text nameLabel;
        public Text priceText;
        public Text stockText;

		public GameObject increasePriceBtn;
		public GameObject decreasePriceBtn;

		protected override void SetupButton()
        {

			nameLabel.text = ItemDetails.Item.itemDetails.itemName;
            priceText.text = ItemDetails.Price.ToString();
            stockText.text = ItemDetails.Number.ToString();

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
			GameObject.FindObjectOfType<ShopScrollView>().IncreasePrice(ItemDetails);
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

			GameObject.FindObjectOfType<ShopScrollView>().DecreasePrice(ItemDetails);
			UpdateButtonsPrice(ItemDetails.Price-1);
		}

		/// <summary>
		/// To refresh data of all the shop buttons
		/// It is useful to update the price text if the same type of sword is present in both inventory column and shop column and if the price is changed
		/// </summary>
		/// <param name="newPrice">New price to update</param>
		public void UpdateButtonsPrice(int newPrice)
		{
			foreach (ShopInventoryScrollButton btn in GameObject.FindObjectsOfType<ShopInventoryScrollButton>())
			{
				if(ItemDetails.Item.itemDetails.itemName == btn.ItemDetails.Item.itemDetails.itemName)
				{
					btn.ItemDetails.Price = newPrice;
					btn.priceText.text = ItemDetails.Price.ToString();
				}
			}
		}
	}
}
