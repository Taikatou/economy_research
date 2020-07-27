using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.ShopUI
{
    public class UpdateItemUi : MonoBehaviour
    {
        public Button moveToMarketplaceButton;
        public Button backButton;
        public Button saveButton;
        public InputField newPriceField;

        private ShopItem _shopItem;
        private MarketPlace _marketPlace;
        private ShopAgent _seller;

        private void Start()
        {
            moveToMarketplaceButton.onClick.AddListener(MoveToMarketPlace);
            backButton.onClick.AddListener(CloseUi);
            saveButton.onClick.AddListener(SaveButton);
        }

        public void SetVisible(ShopItem item, MarketPlace marketPlace, ShopAgent seller)
        {
            _shopItem = item;
            gameObject.SetActive(true);

            _marketPlace = marketPlace;
            _seller = seller;
            newPriceField.text = item.price.ToString();
        }

        private void CloseUi()
        {
            _marketPlace.Refresh();
            gameObject.SetActive(false);
        }

        private void MoveToMarketPlace()
        {
            SaveButton();
            _marketPlace.TransferToShop(_shopItem, _seller);
            CloseUi();
        }

        private void SaveButton()
        {
            _marketPlace.Refresh();
            _shopItem.price = int.Parse(newPriceField.text);
        }
    }
}
