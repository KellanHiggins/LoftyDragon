#if UNITY_4_6
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Bindings
{
		[RequireComponent(typeof(Text))]
		public class UITextPropertyAttachment : MonoBehaviour
		{
				[SerializeField]
				private List<string>
						bindings = new List<string> ();
	
				private List<LabelProperty> properties = new List<LabelProperty> ();
				[SerializeField]
				private string
						format = "{0}";
				Text label;
				public LabelProperty GetProperty (int index)
				{
						if (index >= 0 && index < properties.Count) {
								return properties [index];
						}
						return null;
				}
				System.Object[] values;
				// Use this for initialization
				void Start ()
				{
						values = new System.Object[bindings.Count];
						label = GetComponent<Text> ();
						foreach (string s in bindings) {
								LabelProperty p = new LabelProperty (s);
								p.AddListener (OnValueChange);
								properties.Add (p);
						}
						OnValueChange (null);
				}
				void OnValueChange (System.Object value)
				{
						for (int i = 0; i<values.Length; i++) {
								values [i] = properties [i].value;
						}
						label.text = string.Format (format, values);
				}

		}
}
#endif