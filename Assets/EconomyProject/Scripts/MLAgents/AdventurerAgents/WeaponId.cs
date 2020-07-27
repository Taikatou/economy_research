using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class WeaponId
    {
        private readonly List<string> _weaponList;

        public WeaponId()
        {
            _weaponList = new List<string>();
        }

        private int _GetWeaponId(string itemName)
        {
            bool contains = _weaponList.Contains(itemName);
            if (!contains)
            {
                _weaponList.Add(itemName);
            }

            return _weaponList.IndexOf(itemName);
        }

        private static WeaponId _instance;

        private static WeaponId Instance => _instance ?? (_instance = new WeaponId());

        public static int GetWeaponId(string itemName)
        {
            return Instance._GetWeaponId(itemName);
        }
    }
}
