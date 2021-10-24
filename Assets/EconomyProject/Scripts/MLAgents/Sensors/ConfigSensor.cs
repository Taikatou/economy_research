using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts;
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
    
    public override string GetName() => "ConfigSensor";
    protected override float[] Data => _data;

    public override void Update()
    {
        
    }

    private List<ObsData> GetData(Dictionary<string, int> items)
    {
        var data = new List<ObsData>();
        foreach (var i in items)
        {
            data.AddRange(new []
            {
                new SingleObsData
                {
                    data = i.Value,
                    Name="Price"
                },
                WeaponUtils.GetObsData(i.Key, "weapon name")
            });
        }
        return data;
    }

    private void UpdateData()
    {
        var data = new List<ObsData>();  
        /*var resourceParams = _configSystem.listConfigResources.GetParameters();
        var listConfigItems = _configSystem.listConfigItems.GetParameters();
        foreach (var i in listConfigItems)
        {
            data.AddRange(new ObsData []
            {
                new SingleObsData
                {
                    data = i.price,
                    Name="Price"
                },
                WeaponUtils.GetObsData(i.item.name, "weapon name")
            });
        }

        data.AddRange(ConfigSensorUtils<ECraftingResources>.GetData(resourceParams));*/
        
        //var listDisabilities = _configSystem.listConfigItems.GetDefaultDurabilities();
        // data.AddRange(GetData(listDisabilities));

        var listCraft = _configSystem.listConfigCraft.GetParameters();
        foreach (var craft in listCraft)
        {
            data.Add(
                new CategoricalObsData<ECraftingChoice>(craft.choice)
                {
                    Name = "Choice",
                }
            );
            foreach (var item in craft.resource.resourcesRequirements)
            {
                data.AddRange(new ObsData [] {
                    new SingleObsData
                    {
                        data= item.number / 20,
                        Name="Resource"
                    },
                    new CategoricalObsData<ECraftingResources>(item.type)
                    {
                        Name="item type",
                    }
                });
            }
        }
        
        
        
        _data = ObsData.GetEnumerableData(data);
        MObservationSpec = ObservationSpec.Vector(_data.Length);
    }

    public ConfigSensor(ConfigSystem configSystem) : base()
    {
        _configSystem = configSystem;
        UpdateData();
    }
}
