using EconomyProject.Scripts.Inventory;
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

        private UsableItemDetails _shopDetails; 
        private ShopAgent _seller;

        private void Start()
        {
            moveToMarketplaceButton.onClick.AddListener(MoveToMarketPlace);
            backButton.onClick.AddListener(CloseUi);
            saveButton.onClick.AddListener(SaveButton);
        }

        public void SetVisible(UsableItemDetails details, ShopAgent seller)
        {
            _shopDetails = details;
            gameObject.SetActive(true);

            _seller = seller;
            newPriceField.text = details.ToString();
        }

        private void CloseUi()
        {
            gameObject.SetActive(false);
        }

        private void MoveToMarketPlace()
        {
            SaveButton();
            //_marketPlace.TransferToShop(_shopItem, _seller);
            CloseUi();
        }

        private void SaveButton()
        {
            //_shopDetails.price = int.Parse(newPriceField.text);
        }
    }
}
