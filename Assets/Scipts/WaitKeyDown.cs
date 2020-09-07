using UnityEngine;

public sealed partial class DiceGame
{
	private sealed class WaitKeyDown : CustomYieldInstruction
	{
		KeyCode key;
		public WaitKeyDown(KeyCode key)
		{
			this.key = key;
		}

		public override bool keepWaiting
		{
			get
			{
				return !Input.GetKeyDown(key);
			}
		}
	}
}
