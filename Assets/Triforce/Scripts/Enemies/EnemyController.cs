using UnityEngine;
using System.Collections;

public class EnemyController : GameController {

    public Enemy enemy;

    public int HitPoints { get { return enemy.hitPoints; } }
    public int DamageMin { get { return enemy.weapon.damageMin; } }
    public int DamageMax { get { return enemy.weapon.damageMax; } }
    
}
