using UnityEngine;
using System.Collections;
namespace BellaProject
{
	public static class GameMode
	{
		public enum GameModes
		{
				Normal,
				DebugMode
		}
		public static GameModes gameMode = GameModes.Normal;
	}
}