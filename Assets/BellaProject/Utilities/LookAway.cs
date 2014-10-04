using UnityEngine;
using System.Collections;

public class LookAway : MonoBehaviour
{

		[SerializeField]
		GameObject
				target;

		protected void Update ()
		{
				transform.rotation = target.transform.rotation;
		}
}
