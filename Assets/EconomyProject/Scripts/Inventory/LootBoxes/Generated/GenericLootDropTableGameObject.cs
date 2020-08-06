using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.GameEconomy;

namespace EconomyProject.Scripts.Inventory.LootBoxes.Generated
{
    [System.Serializable]
    public class GenericLootDropTableGameObject : GenericLootDropTable<GeneratedLootItemScriptableObject, UsableItem>
    {
        public List<ItemMap> itemMap;

        private readonly ItemMapper _itemMapper;

        public GenericLootDropTableGameObject() : base()
        {
            _itemMapper = new ItemMapper(itemMap);
        }
            
        public float MaxMoney => lootDropItems.Max(x => _itemMapper.GetValue(x.item));
    }
}
