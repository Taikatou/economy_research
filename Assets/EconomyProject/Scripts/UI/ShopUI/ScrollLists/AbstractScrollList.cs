using System;
using System.Collections.Generic;
using EconomyProject.Scripts.UI.ShopUI.Buttons;
using UnityEngine;

namespace EconomyProject.Scripts.UI.ShopUI.ScrollLists
{

    public interface IScrollList <T>
    {
        void SelectItem(T item, int number = 1);
    }

    public interface ILastUpdate
    {
        DateTime LastUpdated { get; }
        void Refresh();
    }

    public class LastUpdate : MonoBehaviour, ILastUpdate
    {
        public DateTime LastUpdated { get; private set; }
        public virtual void Refresh()
        {
            LastUpdated = DateTime.Now;
        }
    }
    
    public class LastUpdateClass : ILastUpdate
    {
        public DateTime LastUpdated { get; private set; }
        public void Refresh()
        {
            LastUpdated = DateTime.Now;
        }
    }

    public abstract class AbstractScrollList<T, TQ> : MonoBehaviour, IScrollList<T>  where TQ : SampleButton<T>
    {
        public Transform contentPanel;
        public SimpleObjectPool buttonObjectPool;

        private DateTime _lastUpdated;

        protected abstract List<T> GetItemList();
        protected abstract ILastUpdate LastUpdated { get; }

        public abstract void SelectItem(T item, int number=1);

        protected List<TQ> buttons;

        public void Start()
        {
            buttons = new List<TQ>();
        }

        private bool RefreshDisplay()
        {
            RemoveButtons();
            return AddButtons();
        }

        private void RemoveButtons()
        {
            buttons.Clear();
            while (contentPanel.childCount > 0)
            {
                var toRemove = contentPanel.GetChild(0).gameObject;
                buttonObjectPool.ReturnObject(toRemove);
            }
        }

        private bool AddButtons()
        {
            var itemList = GetItemList();
            var valid = itemList != null;
            if (valid)
            {
                foreach (var item in itemList)
                {
                    var newButton = buttonObjectPool.GetObject();
                    newButton.transform.SetParent(contentPanel);

                    var sampleButton = newButton.GetComponent<TQ>();
                    sampleButton.Setup(item, this);
                    
                    buttons.Add(sampleButton);
                }
            }

            return valid;
        }

        protected virtual void Update()
        {
			if(LastUpdated == null || LastUpdated.LastUpdated == null)
			{
				return;
			}

			if (_lastUpdated != LastUpdated.LastUpdated)
            {
                var valid = RefreshDisplay();
                if (valid)
                {
                    _lastUpdated = LastUpdated.LastUpdated;
                }
            }
        }
    }
}