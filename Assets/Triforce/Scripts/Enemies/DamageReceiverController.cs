using UnityEngine;
using System.Collections;

public class DamageReceiverController : GameController {

	public virtual IDamageReceiver damageReceiver { get { return null; } }

}
