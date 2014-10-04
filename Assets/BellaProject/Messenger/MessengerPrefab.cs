using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


public class MessengerPrefab : MonoBehaviour
{
		[HideInInspector]
		public int
				indexLocation;
		[HideInInspector]
		public string
				field;
		public void Publish (Object sender, float num1 =0, float num2 =0, float num3 =0, float num4 =0)
		{
				Manager.messenger.Publish (sender, field, num1, num2, num3, num4);
		}
		public void Subscribe (Del del, bool persistant =false)
		{
				Manager.messenger.Subscribe (field, del, persistant);
		}
		public void Unubscribe (Del del, bool persistant =false)
		{
				Manager.messenger.Unsubscribe (field, del);
		}
		public void SetField (string stringNew)
		{
				field = stringNew;
		}
	
}
public partial class Messages
{
		public const string messengerTest = "messengerTest";
		public const string messengerTest2 = "messengerTest2";
		public static Messages ins = new Messages ();
		public static void RefreshFieldInfo ()
		{
				FieldInfo[] infoArray = typeof(Messages).GetFields ();
				info = new List<FieldInfo> ();
				foreach (FieldInfo f in infoArray) {
						if (f.FieldType == typeof(string)) {
								info.Add (f);
						}
				}
				info.Sort (delegate(FieldInfo x, FieldInfo y) {
						if (x.Name == null && y.Name == null)
								return 0;
						else if (x.Name == null)
								return -1;
						else if (y.Name == null)
								return 1;
						else
								return x.Name.CompareTo (y.Name);
				});
				
		}
		public static List<FieldInfo> fields {
				get { 
						if (info == null) {
								RefreshFieldInfo ();
						}
						return info;
				}
		}
		private static string[] GetFieldNames ()
		{
				string[] names = new string[fields.Count];
				for (int i = 0; i<fields.Count; i++) {
						names [i] = fields [i].Name;
				}
				return names;
		}
		#if UNITY_EDITOR
		public static int InspectorFieldInfoPopUp (string current, int index, ChangeString del)
		{

				string[] names = GetFieldNames ();
				int choice = 0;
		
				GUILayout.BeginHorizontal ();
				choice = UnityEditor.EditorGUILayout.Popup ("Message", index, names);
//				if (GUILayout.Button ("Refresh")) {
//						Messages.RefreshFieldInfo ();
//				}
				GUILayout.EndHorizontal ();
				string selected = (string)fields [choice].GetValue (Messages.ins);
				if (choice >= 0 && !selected.Equals (current) && del != null) {
						del (selected);
						return choice;
				}
				return index;
		}
#endif
		private static  List<FieldInfo> info;
		public delegate void ChangeString (string stringNew);

}
