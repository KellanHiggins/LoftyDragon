using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(MessengerPrefab))]
public class MessagePrefabEditor : Editor
{
		public override void OnInspectorGUI ()
		{
				base.OnInspectorGUI ();
				MessengerPrefab myTarget = target as MessengerPrefab;
//				if (!Application.isPlaying)
				myTarget.indexLocation = Messages.InspectorFieldInfoPopUp (myTarget.field, myTarget.indexLocation, myTarget.SetField);
		}
		
}
