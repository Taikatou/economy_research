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

		private int _currentIndex;

		public Image iconImage;

		public void UpdateData(int currentIndex)
		{
			_currentIndex = currentIndex;
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

		protected override bool Selected()
		{
			return _currentIndex == ItemDetails.Index;
		}
	}
}
