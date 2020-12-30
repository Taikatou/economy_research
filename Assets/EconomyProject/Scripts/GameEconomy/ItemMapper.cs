using System;
using System.Collections.Generic;
using Inventory;

namespace EconomyProject.Scripts.GameEconomy
{
    [Serializable]
    public struct ItemMap
    {
        public UsableItem item;
        public float map;
    }

    public class ItemMapper
    {
        private readonly Dictionary<UsableItem, float> _map;

        public ItemMapper(List<ItemMap> itemMaps)
        {
            _map = new Dictionary<UsableItem, float>();
            foreach (var item in itemMaps)
            {
                _map.Add(item.item, item.map);
            }
        }

        public float GetValue(UsableItem item)
        {
            if (_map.ContainsKey(item))
            {
                return _map[item];
            }
            return 0.0f;
        }
    }
}
