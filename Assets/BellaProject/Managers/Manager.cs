using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public abstract class Manager:MonoBehaviour
{
		protected abstract void Awake ();
		protected abstract void OnDestroy ();
	public static PEPDataPlugin BellaAndroidDataPlugIn;

		public static Messenger messenger {
				get {
						if (messengerInstance == null) {
								messengerInstance = ScriptableObject.CreateInstance<Messenger> ();
						}
						return messengerInstance;
				}
		}
		private static Messenger messengerInstance;
		
		private static Dictionary<System.Type,Manager> managers = new Dictionary<System.Type,Manager > ();
		public static void Register<T> (T manager)where T:Manager
		{
				if (managers.ContainsKey (typeof(T))) {
						if (managers [typeof(T)] != manager) {
								UnityEngine.Object.Destroy (manager);
						}
				} else {
						Debug.Log ("Adding " + manager);
						managers.Add (typeof(T), manager);
				}
		}
		public static void Unregister<T> ()where T:Manager
		{
				if (managers.ContainsKey (typeof(T))) {
						managers.Remove (typeof(T));
				} 
		}
		public static T Get<T> ()where T:Manager
		{
				if (managers.ContainsKey (typeof(T))) {
						return managers [typeof(T)] as T;
				} 
				return null;
		}
}
public abstract class GenericManager<T>:Manager where T:Manager
{
		protected override sealed void Awake ()
		{
				T value = this as T;
				Register<T> (value);
				AwakeNew ();
		}
		protected virtual void AwakeNew ()
		{			
		}
			
		protected override sealed void OnDestroy ()
		{
				if (Get<T> () == this)
						Unregister<T> ();
				OnDestroyNew ();
		}
		protected virtual void OnDestroyNew ()
		{
		}
}
