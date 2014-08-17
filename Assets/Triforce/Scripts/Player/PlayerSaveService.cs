using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerSaveService {

    Player playerRef;

    List<PlayerSaveProperty> saveProps = new List<PlayerSaveProperty> {
        new PlayerSaveProperty("HitPoints", "int", 100),
        new PlayerSaveProperty("MaxHitPoints", "int", 100),
        new PlayerSaveProperty("Mana", "int", 100),
        new PlayerSaveProperty("MaxMana", "int", 100),
        new PlayerSaveProperty("Arrows", "int", 5),
        new PlayerSaveProperty("MaxArrows", "int", 5),
        new PlayerSaveProperty("Lockpicks", "int", 0),
        new PlayerSaveProperty("Runes", "int", 5),
        new PlayerSaveProperty("MaxRunes", "int", 5)
    };

    public void Load (ref Player player) {
        playerRef = player;
        foreach (PlayerSaveProperty prop in saveProps) {
            LoadProperty(prop);
        }
    }

    void LoadProperty (PlayerSaveProperty prop) {
        Debug.Log("HItpoints type is " + playerRef.HitPoints.GetType());
        if (ES2.Exists(prop.Name)) {
            var value = ES2.Load<int>(prop.Name);
            playerRef.SetProperty(prop.Name, value);
        } else {
            ES2.Save<object>(prop.DefaultValue, prop.Name);
        }
    }

}
