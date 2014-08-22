using UnityEngine;
using System.Collections;
using System;

public class PlayerSaveProperty {

    public string Name { get; set; }
    public string PropType { get; set; }
    public object DefaultValue { get; set; }

    public PlayerSaveProperty (string name, string type, object defaultValue) {
        Name = name;
        PropType = type;
        DefaultValue = defaultValue;
    }


}
