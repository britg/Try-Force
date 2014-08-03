using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Call Add of HUDText.")]
	public class SetUIFollowTarget : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;
		
		public FsmOwnerDefault target;

		UIFollowTarget followTarget;
		
		
		public override void Reset()
		{
			
			target = null;
			
		}
		
		public override void OnEnter()
		{
			GameObject	go  = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}


			if (followTarget == null)
			{
				followTarget = go.GetComponent<UIFollowTarget>();
			}

			if (followTarget == null) {
				return;
			}

			GameObject targetObj = Fsm.GetOwnerDefaultTarget(target);
			followTarget.target = targetObj.transform;

			Finish();
		}
		
		
	}
}