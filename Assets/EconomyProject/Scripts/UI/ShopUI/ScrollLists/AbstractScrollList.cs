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

    public class LastUpdate : MonoBehaviour
    {
        public DateTime LastUpdated { get; private set; }
        public void Refresh()
        {
            LastUpdated = DateTime.Now;;
        }
    }

    public abstract class AbstractScrollList<T, TQ> : MonoBehaviour, IScrollList<T>  where TQ : SampleButton<T>
    {
        public Transform contentPanel;
        public SimpleObjectPool buttonObjectPool;

        private DateTime _lastUpdated;

        public abstract List<T> ItemList { get; }
        public abstract LastUpdate LastUpdated { get; }

        public abstract void SelectItem(T item, int number=1);

        public virtual bool RefreshDisplay()
        {
            RemoveButtons();
            return AddButtons();
        }

        public void RemoveButtons()
        {
            while (contentPanel.childCount > 0)
            {
                var toRemove = contentPanel.GetChild(0).gameObject;
                buttonObjectPool.ReturnObject(toRemove);
            }
        }

        protected bool AddButtons()
        {
            var valid = ItemList != null;
            if (valid)
            {
                foreach (var item in ItemList)
                {
                    var newButton = buttonObjectPool.GetObject();
                    newButton.transform.SetParent(contentPanel);

                    var sampleButton = newButton.GetComponent<TQ>();
                    sampleButton.Setup(item, this);
                }
            }

            return valid;
        }

        private void Update()
        {
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