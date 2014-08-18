using UnityEngine;
using System.Collections;

[System.Serializable]
public class Enemy : IDamageable {

    public int hitPoints;
    public int maxHitPoints;
    public int armor;

    public int HitPoints { 
        get {
            return hitPoints;
        }
        set {
            hitPoints = value;
        }
    }

    public int MaxHitPoints { 
        get {
            return maxHitPoints;
        }
        set {
            maxHitPoints = value;
        }
    }

    public int Armor {
        get {
            return armor;
        }
        set {
            armor = value;
        }
    }

    public bool Dead { get; set; }


    public Weapon weapon;

    public void TakeDamage (int amount) {
        int newHp = HitPoints - amount;
        HitPoints = (int)Mathf.Clamp((float)newHp, 0f, (float)MaxHitPoints);
    }

}
