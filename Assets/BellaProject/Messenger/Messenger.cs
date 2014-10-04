using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Messenger : ScriptableObject
{
		Dictionary<string,List<DelegatePackage>> listnerList = new Dictionary<string, List<DelegatePackage>> ();


		public Messenger ()
		{
				Object.DontDestroyOnLoad (this);
		}

		
		public void Subscribe (string msgId, Del callback, bool persistant =false)
		{
				if (listnerList.ContainsKey (msgId)) {
						listnerList [msgId].Add (new DelegatePackage (callback, persistant));
				} else {
						List<DelegatePackage> list = new List<DelegatePackage> ();
						list.Add (new DelegatePackage (callback, persistant));
						listnerList.Add (msgId, list);
				}
		}
		public void Unsubscribe (string msgId, Del callback)
		{
				if (listnerList.ContainsKey (msgId)) {
						for (int i = 0; i< listnerList [msgId].Count; i++) {
								if (listnerList [msgId] [i] != null) {
										if (listnerList [msgId] [i].method.Equals (callback)) {
												listnerList [msgId] [i] = null;
										}
								}
						}

			
				}
		}
		List<DelegatePackage> toRemove = new List<DelegatePackage> ();
		public bool Publish (Object sender, string msgId, float num1 =0, float num2 =0, float num3 =0, float num4 =0)
		{
				if (listnerList.ContainsKey (msgId)) {
						for (int i = 0; i< listnerList [msgId].Count; i++) {
								if (listnerList [msgId] [i] != null) {
										listnerList [msgId] [i].method.Invoke (sender, msgId, num1, num2, num3, num4);
								} else {
										toRemove.Add (listnerList [msgId] [i]);
								}
								
						}
						foreach (DelegatePackage package in toRemove) {
								listnerList [msgId].Remove (package);
						}
						return true;
				}
				return false;
		}
		
 
}
public delegate void Del (Object sender,string msgID,float num1 =0,float num2 =0,float num3 =0,float num4 =0);
class DelegatePackage
{
		public Del method;
		public bool persistant;
		public DelegatePackage (Del method, bool persistant)
		{
				this.persistant = persistant;
				this.method = method;
		}
}