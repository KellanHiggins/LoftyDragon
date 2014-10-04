using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BindingsCleaner : MonoBehaviour
{
		public const string CleanupAllPropertiesAndBindings = "CleanupAllPropertiesAndBindings";
		private static List<CleanUpMethod> methods = new List<CleanUpMethod> ();
		public static void Add (CleanUpMethod method)
		{
				methods.Add (method);
		}
		public delegate void CleanUpMethod ();
		public static BindingsCleaner Instance {
				get { 
						if (ins == null) {
								ins = new GameObject ("_BindingsCleaner").AddComponent<BindingsCleaner> ();
						}
						return ins;
				}
		}
		public static bool Check ()
		{
				if (ins == null) {
						ins = new GameObject ("_BindingsCleaner").AddComponent<BindingsCleaner> ();
				}

				return ins != null;
		}

		void OnDestroy ()
		{
				foreach (CleanUpMethod m in methods) {
						m ();
				}
				methods = new List<CleanUpMethod> ();
		}
		private static BindingsCleaner ins;
}
