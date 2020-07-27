using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.UI.Adventurer;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI
{
    public class AuctionMenu : MonoBehaviour
    {
        public Slider slider;

        public Text itemText;

        public UiAccessor accessor;
        
        private GameAuction GameAuction => accessor.GameAuction;

        private void Update()
        {
            if (GameAuction.ItemCount > 0 && GameAuction.auctionedItem)
            {
                slider.value = Progress;
                itemText.text = "Name: " + GameAuction.auctionedItem.itemName +
                                " efficiency" + GameAuction.auctionedItem.efficiency +
                                " Bid price: " + GameAuction.currentItemPrice;
            }
            else
            {
                itemText.text = "";
            }
        }

        public float Progress => GameAuction.Progress;
    }
}
