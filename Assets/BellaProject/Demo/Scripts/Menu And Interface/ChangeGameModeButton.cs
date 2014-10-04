using UnityEngine;
using System.Collections;
using BellaProject;
public class ChangeGameModeButton : MonoBehaviour
{
		[SerializeField]
		private GameMode.GameModes
				gameModeOnPress = GameMode.GameModes.Normal;
		public void OnButtonPress ()
		{
				GameMode.gameMode = gameModeOnPress;
		}
}
