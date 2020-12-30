using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using NUnit.Framework;

namespace Tests.Economy
{
    public class CraftingSystemTests
    {
        [Test]
        public void TestCrafting()
        {
            var shop = new ShopCraftingSystem
            {
                craftingSubSubSystem = new CraftingSubSystem(),
                shopSubSubSystem = new AgentShopSubSystem()
            };
            
            // shop.
        }
    }
}
