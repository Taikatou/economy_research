using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.ShopUI.Buttons
{
    public abstract class SampleButton <T> : MonoBehaviour
    {
        public Button buttonComponent;

        private IScrollList<T> _scrollList;

        protected T ItemDetails;

        // Use this for initialization
        private void Start()
        {
            buttonComponent.onClick.AddListener(HandleClick);
        }

        public void Setup(T currentItemDetails, IScrollList<T> currentScrollList)
        {
            ItemDetails = currentItemDetails;
            _scrollList = currentScrollList;
            SetupButton();
        }

        protected abstract void SetupButton();

        private void HandleClick()
        {
            var number = 1;
            _scrollList.SelectItem(ItemDetails, number);
        }
    }
}