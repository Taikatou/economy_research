using System;
using System.Linq;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using Inventory;

public class CraftingRequestLocationMap : LocationSelect<ShopAgent>
{
    public ShopCraftingSystem shopCraftingSystem { get; set; }
    public override int GetLimit(ShopAgent agent)
    {
        var shopItems = shopCraftingSystem.craftingSubSubSystem.craftingRequirement;
        return shopItems.Count;
    }

    public ECraftingChoice GetCraftingChoice(ShopAgent agent)
    {
        var location = GetCurrentLocation(agent);
        var valuesAsList = Enum.GetValues(typeof(ECraftingChoice)).Cast<ECraftingChoice>().ToList();
        return valuesAsList[location];
    }
}
