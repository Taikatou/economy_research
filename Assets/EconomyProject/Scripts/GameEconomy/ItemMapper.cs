using System;
using System.Collections.Generic;
using EconomyProject.Scripts.Inventory;

namespace EconomyProject.Scripts.GameEconomy
{
    [Serializable]
    public struct ItemMap
    {
        public InventoryItem item;
        public float map;
    }

    public class ItemMapper
    {
        private readonly Dictionary<InventoryItem, float> _map;

        public ItemMapper(List<ItemMap> itemMaps)
        {
            _map = new Dictionary<InventoryItem, float>();
            foreach (var item in itemMaps)
            {
                _map.Add(item.item, item.map);
            }
        }

        public float GetValue(InventoryItem item)
        {
            if (_map.ContainsKey(item))
            {
                return _map[item];
            }
            return 0.0f;
        }
    }
}
