using System;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.ShopUI.Buttons
{
    public abstract class SampleButton <T> : MonoBehaviour
    {
        public Image image;
        public Color selectedColor = Color.green;
        public Color unselectedColor = Color.white;
        
        public Button buttonComponent;
        private IScrollList<T> _scrollList;
        protected T ItemDetails;
        
        protected abstract void SetupButton();

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

        private void HandleClick()
        {
            var number = 1;
            _scrollList.SelectItem(ItemDetails, number);
        }

        public void Update()
        {
            image.color = Selected() ? selectedColor : unselectedColor;
        }
        
        protected virtual bool Selected()
        {
            return false;
        }
    }
}