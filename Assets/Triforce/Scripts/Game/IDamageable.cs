using UnityEngine;
using System.Collections;

public interface IDamageable {

    int HitPoints { get; set; }
    int MaxHitPoints { get; set; }
    int Armor { get; set; }
    bool Dead { get; set; }
}
