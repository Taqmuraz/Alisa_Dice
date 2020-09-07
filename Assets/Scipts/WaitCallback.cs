using UnityEngine;

public sealed partial class DiceGame
{
	private sealed class WaitCallback : CustomYieldInstruction
	{
		bool wait = true;

		public void Callback()
		{
			wait = false;
		}

		public override bool keepWaiting { get { return wait; } }
	}
}
