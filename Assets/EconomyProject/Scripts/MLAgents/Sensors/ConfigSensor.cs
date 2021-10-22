using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

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
public class ConfigSensor : BaseEconomySensor
{
    // Update is called once per frame
    protected override float[] Data => _data;
    private float[] _data;

    public override string GetName() => "ConfigSensor";

    public override void Update()
    {
        UpdateData();
    }

    private readonly ConfigSystem _configSystem;
    
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
                    data= NameHashTable[i.Key],
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
        var listDurabilities = _configSystem.listConfigItems.GetDefaultDurabilities();
        var listDamages = _configSystem.listConfigItems.GetDefaultDamages();
        var data = new List<ObsData>();    
        data.AddRange(ConfigSensorUtils<ECraftingResources>.GetData(resourceParams));
        data.AddRange(GetData(listDurabilities));
        data.AddRange(GetData(listDamages));


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
                    data= NameHashTable[i.item.name],
                    name="name"
                }
            });
        }
        
        _data = new float[data.Count];
        var counter = 0;
        foreach (var o in data)
        {
            _data[counter++] = o.data;
        }
        MObservationSpec = ObservationSpec.Vector(data.Count);
    }

    public ConfigSensor(ConfigSystem configSystem) : base()
    {
        _configSystem = configSystem;
        UpdateData();
    }

    private readonly Dictionary<string, int> NameHashTable = new Dictionary<string, int>
    {
        {"Beginner Sword", 0},
        {"Intermediate Sword", 1},
        {"Advanced Sword", 2},
        {"Epic Sword", 3},
        {"Master Sword", 4},
        {"Ultimate Sword", 5}
    };
}
