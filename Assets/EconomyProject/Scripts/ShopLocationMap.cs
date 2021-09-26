
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.Inventory;

public class ShopLocationMap : LocationSelect<ShopAgent>
{
    public ShopCraftingSystem craftingSystem { get; set; }
    // Start is called before the first frame update
    protected override int GetLimit(ShopAgent agent)
    {
        var items = GetData(agent);
        return items.Count;
    }

    public List<ShopItem> GetData(ShopAgent agent)
    {
        var itemList = new List<ShopItem>();
        foreach (var item in agent.agentInventory.Items)
        {
            var price = craftingSystem.shopSubSubSystem.GetPrice(agent, item.Value[0].itemDetails);
            itemList.Add(new ShopItem { Item=item.Value[0], Number=item.Value.Count, Price=price});
        }

        return itemList;
    }
    
    public ShopItem GetCraftingChoice(ShopAgent agent)
    {
        var items = GetData(agent);
        return items[GetCurrentLocation(agent)];
    }
}
