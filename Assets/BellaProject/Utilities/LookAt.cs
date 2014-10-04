using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour
{
		[SerializeField]
		GameObject
				target;
	
		// Update is called once per frame
		void Update ()
		{
				transform.LookAt (target.transform);
		}
}
