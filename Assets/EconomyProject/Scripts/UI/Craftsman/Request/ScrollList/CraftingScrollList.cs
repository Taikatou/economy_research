using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.ShopUI.Buttons;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Craftsman.Request.ScrollList
{
    public abstract class CraftingScrollList<T, TQ> : AbstractScrollList<T, TQ> where TQ : SampleButton<T>
    {
        protected override LastUpdate LastUpdated => requestSystem.requestSystem;

        public RequestShopSystem requestSystem;
    }
}
