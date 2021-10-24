using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;
using UnityEngine;

public static class ConfigSensorUtils<T> where T : Enum
{
    public static List<ObsData> GetData(Dictionary<T, int> values)
    {
        var data = new List<ObsData>();
        foreach (var v in values)
        {
            data.AddRange(new[]
            {
                new ObsData
                {
                    data = Convert.ToInt32(v.Key),
                    name = "Resource"
                },
                new ObsData
                {
                    data = v.Value,
                    name = "Value"
                }
            });
        }

        return data;
    }
}

public static class WeaponUtils
{
    public static readonly Dictionary<string, int> NameHashTable = new Dictionary<string, int>
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
                new ObsData
                {
                    data = i.Value,
                    name="Price"
                },
                new ObsData
                {
                    data= WeaponUtils.NameHashTable[i.Key],
                    name="name"
                }
            });
        }
        return data;
    }

    private void UpdateData()
    {
        var resourceParams = _configSystem.listConfigResources.GetParameters();
        var listConfigItems = _configSystem.listConfigItems.GetParameters();
        var listDisabilities = _configSystem.listConfigItems.GetDefaultDurabilities();
        var data = new List<ObsData>();    
        
        data.AddRange(ConfigSensorUtils<ECraftingResources>.GetData(resourceParams));
        data.AddRange(GetData(listDisabilities));

        var listCraft = _configSystem.listConfigCraft.GetParameters();
        foreach (var craft in listCraft)
        {
            data.Add(
                new ObsData
                {
                    data = (int) craft.choice,
                    name = "Choice"
                }
            );
            foreach (var item in craft.resource.resourcesRequirements)
            {
                data.AddRange(new [] {
                    new ObsData
                    {
                        data= item.number,
                        name="Resource"
                    },
                    new ObsData
                    {
                        data=(int)item.type,
                        name="item type"
                    }
                });
            }
        }
        foreach (var i in listConfigItems)
        {
            data.AddRange(new []
            {
                new ObsData
                {
                    data = i.price,
                    name="Price"
                },
                new ObsData
                {
                    data= WeaponUtils.NameHashTable[i.item.name],
                    name="name"
                }
            });
        }
        
        var output = "";
        foreach (var item in data)
        {
            output += "\t" + item.name + ": " + item.data;
        }
        Debug.Log(output);
        
        _data = new float[data.Count];
        MObservationSpec = ObservationSpec.Vector(data.Count);
        for (var i = 0; i < data.Count; i++)
        {
            _data[i] = data[i].data;
        }
    }

    public ConfigSensor(ConfigSystem configSystem) : base()
    {
        _configSystem = configSystem;
        UpdateData();
    }
}
