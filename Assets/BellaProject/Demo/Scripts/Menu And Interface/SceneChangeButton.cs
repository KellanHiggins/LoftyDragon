using UnityEngine;
using System.Collections;

public class SceneChangeButton : MonoBehaviour
{
		[SerializeField]
		private int
				sceneIndexToLoad = 0;
		public void OnButtonPress ()
		{
				Application.LoadLevel (sceneIndexToLoad);
		} 
}
