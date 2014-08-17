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

    public int HitPoints { get; set; }
    public int Mana { get; set; }
    public int Arrows { get; set; }
    public int Runes { get; set; }
    public int Lockpicks { get; set; }

    public int Gold { get; set; }

	public float moveSpeed;
	public float rotateSpeed;

	public Player.Orientation orientation = Player.Orientation.Warrior;

	public Warrior warrior;
	public Thief thief;
	public Mage mage;

	public bool warriorFace { get { return orientation == Player.Orientation.Warrior; } }
	public bool thiefFace { get { return orientation == Player.Orientation.Thief; } }
	public bool mageFace { get { return orientation == Player.Orientation.Mage; } }

    public object GetProperty (string propertyName) {
        return this.GetType().GetProperty(propertyName).GetValue(this, null);
    }

}
