using UnityEngine;
using System.Collections;

[System.Serializable]
public class Weapon {

    public string name;
    public DamageType damageType;

    public int damageMin;
    public int damageMax;
    public float critChance;

    public override string ToString () {
        return "[Weapon: " + name + "]";
    }
}
