using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Call Add of HUDText.")]
	public class ShowHUDText : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(HUDText))]
		public FsmOwnerDefault gameObject;
		
		HUDText hudText;
		public FsmString textVal;
		public FsmFloat floatVal;
		public FsmInt intVal;
		public FsmColor c;
		public FsmFloat stayDuration;
		
		
		public override void Reset()
		{

			textVal = null;
			floatVal = null;
			intVal = null;
			c = null;
			stayDuration = null;
			
		}
		
		public override void OnEnter()
		{
			// check that we have MyScript referenced
			
			GameObject	go  = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}
			

			if (hudText == null)
			{
				hudText = go.GetComponent<HUDText>();
			}

			string stringVal = "";

			if (!textVal.IsNone) {
				stringVal = textVal.Value;
			}

			if (!floatVal.IsNone) {
				stringVal = floatVal.Value.ToString("F0");
			}

			if (!intVal.IsNone) {
				stringVal = intVal.Value.ToString();
			}

			hudText.Add(stringVal, c.Value, stayDuration.Value);

			Finish();
		}
		
		
	}
}