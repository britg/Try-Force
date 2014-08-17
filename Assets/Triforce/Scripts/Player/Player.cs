using UnityEngine;
using System.Collections;

[System.Serializable]
public class Player {

	public enum Sector {
		Top,
		Right,
		Left
	}

	public enum Orientation {
		Warrior,
		Thief,
		Mage
	}

	public int maxHitPoints;
    public int maxMana;
    public int maxArrows;
    public int maxRunes;
    public int maxLockpicks;

	int currentHitPoints;
    int currentMana;
    int currentArrows;
    int currentRunes;
    int currentLockpicks;

    public int gold;

	public float moveSpeed;
	public float rotateSpeed;

	public Player.Orientation orientation = Player.Orientation.Warrior;

	public Warrior warrior;
	public Thief thief;
	public Mage mage;

	public bool warriorFace { get { return orientation == Player.Orientation.Warrior; } }
	public bool thiefFace { get { return orientation == Player.Orientation.Thief; } }
	public bool mageFace { get { return orientation == Player.Orientation.Mage; } }

}
