using UnityEngine;
using System.Collections;

public class PlayerSaveService {

    public static string saveKey = "Triforce.save";

    Player playerRef;

    public void Load(ref Player player) {
        playerRef = player;

        bool saveExists = ES2.Exists(PlayerSaveService.saveKey);
        if (saveExists) {
            LoadSave();
        } else {
            CreateNewSave();
        }
    }

    void LoadSave () {
        Debug.Log("Loading existing save");
        Debug.Log("hitpoints is " + playerRef.GetProperty("HitPoints"));
    }

    void CreateNewSave () {
        Debug.Log("Creating new save");
        ES2.Save<string>("1", PlayerSaveService.saveKey);
    }

}
