// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.
// micro script by Andrew Raphael Lukasik

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Level)]
	[Tooltip("Returns current level's name as a string.")]
	public class LevelNameToString : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString stringVariable;
		[RequiredField]
		private FsmString stringValue;
		
		public override void Reset()
		{
			stringVariable = null;
			stringValue = null;
		}

		public override void OnEnter()
		{
			DoSetStringValue();
			
			Finish();
		}

		
		void DoSetStringValue()
		{
			if (stringVariable == null) return;
			stringValue = Application.loadedLevelName;
			stringVariable.Value = stringValue.Value;
		}
		
	}
}