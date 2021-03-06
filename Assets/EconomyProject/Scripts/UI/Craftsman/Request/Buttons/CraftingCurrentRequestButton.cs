﻿using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Craftsman.Request.Buttons
{
    public class CraftingCurrentRequestButton : SampleButton<CraftingResourceRequest>
    {
        public Text nameLabel;
        public Text stockText;
        public Text price;
		public Image iconImage;

        protected override void SetupButton()
        {
            nameLabel.text = ItemDetails.Resource.ToString();
			iconImage.sprite = ItemDetails.Icon;
			stockText.text = "x" + ItemDetails.Number;
            price.text = ItemDetails.Price.ToString();
        }
    }
}
