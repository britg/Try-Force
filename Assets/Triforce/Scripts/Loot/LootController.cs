using UnityEngine;
using System.Collections;

public class LootController : GameController {

    public void AcquireLoot(string lootClassName)
    {
        Debug.Log("Acquired loot " + lootClassName);
    }
}
