using UnityEngine;
using System.Collections;

public sealed class DiceCube : MonoBehaviour
{
	static readonly int[] positiveAxes = { 6, 3, 5 };
	static readonly int[] negativeAxes = { 1, 4, 2 };

	new Rigidbody rigidbody;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	public void ThrowAndWaitValue(System.Action<int> callback)
	{
		rigidbody.velocity = new Vector3(0f, 0f, Random.Range(0.75f, 2f));
		rigidbody.angularVelocity = Random.insideUnitSphere * 45f;
		StartCoroutine(WaitValueRoutine(callback));
	}
	private IEnumerator WaitValueRoutine(System.Action<int> callback)
	{
		var wait = new WaitForEndOfFrame();
		while (rigidbody.velocity.magnitude > 0f && rigidbody.angularVelocity.magnitude > 0f)
		{
			yield return wait;
		}
		Destroy(rigidbody);

		int cubeValue = 0;

		for (int i = 0; i < 3; i++)
		{
			Vector3 cubeAxis = new Vector3();
			Vector3 axis = Vector3.up;
			cubeAxis[i] = 1f;
			cubeAxis = transform.TransformDirection(cubeAxis);
			float dot = Vector3.Dot(cubeAxis, axis);

			if (dot > 0.6f) cubeValue = positiveAxes[i];
			else if (dot < -0.6f) cubeValue = negativeAxes[i];
			else continue;
			break;
		}

		if (cubeValue == 0) throw new System.Exception("Wrong cube value : " + cubeValue.ToString());
		callback(cubeValue);
	}
}
