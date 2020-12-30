using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.UI.ShopUI.Buttons;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Craftsman.Request.ScrollList
{
    public abstract class ShopRequestScrollList<T, TQ> : AbstractScrollList<T, TQ> where TQ : SampleButton<T>
    {
        public RequestSystem requestSystem;
        protected override ILastUpdate LastUpdated => requestSystem;
    }
}
