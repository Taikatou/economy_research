using System;
using EconomyProject.Scripts.UI;
using TurnBased.Scripts;
using UnityEngine;

namespace EconomyProject
{
    public class PlayerBattleHud : MonoBehaviour
    {
        public GetCurrentAdventurerAgent adventurerAgent;

        public int _cacheIndex = -1;
        public void Update()
        {
            if (_cacheIndex != adventurerAgent.Index)
            {
                
            }
        }
    }
}
