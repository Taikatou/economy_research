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
        
        public GetCurrentAdventurerAgent currentAdventurerAgent;

        private void Update()
        {
            if (GameAuction.ItemCount > 0 && GameAuction.auctionedItem)
            {
                slider.value = Progress;
                itemText.text = "Name: " + GameAuction.auctionedItem.itemDetails.itemName +
                                " efficiency" + GameAuction.auctionedItem.itemDetails.damage +
                                " Bid price: " + GameAuction.currentItemPrice;
            }
            else
            {
                itemText.text = "";
            }
        }

        public float Progress => GameAuction.Progress(currentAdventurerAgent.CurrentAgent);
    }
}
