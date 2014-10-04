using UnityEngine;
using System.Collections;

public class LerpFollower : MonoBehaviour
{
		[SerializeField]
		private GameObject
				target;
		[SerializeField]
		private float
				lerpSpeed;
		[SerializeField]
		private bool
				PerFrame = false;

	
		// Update is called once per frame
		void Update ()
		{
				float speed = lerpSpeed;
				if (!PerFrame) {
						speed *= Time.deltaTime;
				}
				transform.position = Vector3.Lerp (transform.position, target.transform.position, speed);
		}
}
