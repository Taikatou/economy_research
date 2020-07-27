using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Shop
{
    public class MarketPlace : LastUpdate
    {
        private Dictionary<ShopItem, ShopAgent> _sellers;

        public List<ShopItem> ItemList => _sellers != null? _sellers.Keys.ToList() : new List<ShopItem>();

        private void Start()
        {
            _sellers = new Dictionary<ShopItem, ShopAgent>();
            Refresh();
        }

        public int SellersItems(ShopAgent seller)
        {
            var count = 0;
            foreach (var item in ItemList)
            {
                if (item.Seller == seller)
                {
                    count++;
                }
            }
            return count;
        }

        public ShopAgent GetSeller(ShopItem item)
        {
            var contains = _sellers.ContainsKey(item);
            if (contains)
            {
                return _sellers[item];
            }

            return null;
        }

        public void RemoveItem(ShopItem item, int number=1)
        {
            var foundItem = FindItem(item);
            var toRemove = foundItem.DeductStock(number);
            if (toRemove)
            {
                _sellers.Remove(foundItem);
            }

            Refresh();
        }

        public void TryTransferItemToOtherShop(ShopItem item, AgentShopScrollList otherShop)
        {
            var seller = GetSeller(item);
            var canBuy = otherShop.Gold >= item.price;
            var sellerValid = seller != null;


            if (canBuy && sellerValid)
            {
                seller.Wallet.EarnMoney(item.price);
                otherShop.Gold -= item.price;

                var newItem = new ShopItem(item, 1, seller);
                otherShop.ShopAgent.AddItem(newItem);
                RemoveItem(newItem);

                otherShop.RefreshDisplay();
                Debug.Log("enough gold");
            }
            Debug.Log("attempted");
            Refresh();
        }

        public void AddItem(ShopItem item, ShopAgent shopAgent)
        {
            var foundItem = FindItem(item);
            if (foundItem != null)
            {
                foundItem.IncreaseStock(item.stock);
            }
            else
            {
                ItemList.Add(item);
                _sellers.Add(item, shopAgent);
            }
        }

        public ShopItem FindItem(ShopItem item)
        {
            foreach (var i in ItemList)
            {
                var isItem = i.inventoryItem == item.inventoryItem;
                var isPrice = item.price == i.price;
                //Debug.Log(isSeller + "\t" + isItem + "\t" + isPrice);
                if (isItem && isPrice)
                {
                    return i;
                }
            }

            return null;
        }

        public void TransferToShop(ShopItem item, ShopAgent seller, int stockNumber = 1)
        {
            var newItem = new ShopItem(item, stockNumber, seller);
            AddItem(newItem, seller);
            seller.RemoveItem(newItem);
            Refresh();
        }
    }
}
