using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Sensors;
using Inventory;
using Unity.MLAgents.Sensors;
using UnityEngine;

public static class ConfigSensorUtils<T> where T : Enum
{
    public static List<ObsData> GetData(Dictionary<T, int> values)
    {
        var data = new List<ObsData>();
        foreach (var v in values)
        {
            data.AddRange(new ObsData [] 
            {
                new CategoricalObsData<T>(v.Key)
                {
                    Name = "Resource"
                },
                new SingleObsData
                {
                    data = v.Value,
                    Name = "Value"
                }
            });
        }

        return data;
    }
}

public static class WeaponUtils
{
    public static Dictionary<string, int> NameHashTable => new Dictionary<string, int>
    {
        {"Unarmed", 6},
        {"Beginner Sword", 0},
        {"Intermediate Sword", 1},
        {"Advanced Sword", 2},
        {"Epic Sword", 3},
        {"Master Sword", 4},
        {"Ultimate Sword", 5},
        {"1_BeginnerSword", 0},
        {"2_IntermediateSword", 1},
        {"3_AdvancedSword", 2},
        {"4_EpicSword", 3},
        {"5_MasterSword", 4},
        {"6_UltimateSwordOfPower", 5}
    };

    public static ObsData GetObsData(string name, string dataName)
    {
        return new BaseCategoricalObsData(WeaponUtils.NameHashTable[name], 7) {Name=dataName};
    }
}
public class ConfigSensor : BaseEconomySensor
{
    private float[] _data;
    private readonly ConfigSystem _configSystem;

    public override void Update()
    {
        
    }

    public override string GetName() => "ConfigSensor";
    protected override float[] Data => _data;

    private void UpdateData()
    {
        var data = new List<ObsData>();

        var listCraft = _configSystem.listConfigCraft.GetParameters();
        foreach (var item in SensorUtils<ECraftingChoice>.ValuesToArray)
        {
            var length = SensorUtils<ECraftingResources>.Length;
            var itemNumber = new int [length];
            var requirements = listCraft.First(l => l.choice == item);
            for (var i = ECraftingResources.Wood; (int) i < length; i++)
            {
                var index = (int) i;
                foreach (var r in requirements.resource.resourcesRequirements)
                {
                    if (r.type == i)
                    {
                        itemNumber[(int)i] = r.number / 10;
                        break;
                    }
                }
                data.Add(new SingleObsData{data = itemNumber[index], Name = i.ToString()});
            }
        }

        _data = ObsData.GetEnumerableData(data);
        MObservationSpec = ObservationSpec.Vector(_data.Length);
    }

    public ConfigSensor(ConfigSystem configSystem) : base(null)
    {
        _configSystem = configSystem;
        UpdateData();
    }
}
