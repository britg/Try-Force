﻿// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the Angle between a GameObject's up axis and a Target. The Target can be defined as a GameObject or a world Position. If you specify both, then the Position will be used as a local offset from the Target Object's position.")]
	public class GetAngleToTarget2D : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The game object whose forward axis we measure from. If the target is dead ahead the angle will be 0.")]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("The target object to measure the angle to. Or use target position.")]
		public FsmGameObject targetObject;
		
		[Tooltip("The world position to measure an angle to. If Target Object is also specified, this vector is used as an offset from that object's position.")]
		public FsmVector3 targetPosition;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the angle in a float variable.")]
		public FsmFloat storeAngle;
		
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
		
		public override void Reset()
		{
			gameObject = null;
			targetObject = null;
			targetPosition = new FsmVector3 { UseVariable = true};
			storeAngle = null;
			everyFrame = false;
		}
		
		public override void OnLateUpdate()
		{
			DoGetAngleToTarget();
			
			if (!everyFrame)
			{
				Finish();
			}
		}
		
		void DoGetAngleToTarget()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}
			
			var goTarget = targetObject.Value;
			if (goTarget == null && targetPosition.IsNone)
			{
				return;
			}
			
			Vector2 targetPos;
			if (goTarget != null)
			{
				targetPos = !targetPosition.IsNone ? 
					goTarget.transform.TransformPoint(targetPosition.Value).XY() : 
						goTarget.transform.position.XY();
			}
			else
			{
				targetPos = targetPosition.Value.XY ();
			}
			
			var targetDir = targetPos - go.transform.position.XY ();
			storeAngle.Value = Vector2.Angle(targetDir, go.transform.up);
		}
		
	}
}