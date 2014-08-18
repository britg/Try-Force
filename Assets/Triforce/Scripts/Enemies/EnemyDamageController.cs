using UnityEngine;
using System.Collections;

public class EnemyDamageController : GameController {

    Enemy enemy;

    void OnTriggerEnter2D (Collider2D other) {
        if (other.gameObject.tag == Game.weaponTag) {
            Weapon weapon = other.gameObject.GetComponent<WeaponController>().weapon;
            TakeDamage(weapon);
        }
    }

    void TakeDamage (Weapon weapon) {
        enemy = GetComponent<EnemyController>().enemy;
        var service = new DamageCalculatorService(weapon, enemy);
        int damageAmount = service.CalculateDamage();
        enemy.TakeDamage(damageAmount);
    }
    
}
