using UnityEngine;
using System.Collections;

public class ManaPotionController : LootController {

    public void AcquireLoot(float manaAmount)
    {
        Debug.Log("Acquired " + manaAmount + " mana");
    }
}
