#if UNITY_4_6
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Bindings
{
		[RequireComponent(typeof(Text))]
		public class UITextPropertyAttachmentSingle : MonoBehaviour
		{
				public string
						binding;
	
				private LabelProperty  property;
				public LabelProperty GetProperty ()
				{
						return property;
				}
				[SerializeField]
				private string
						format = "{0}";
				Text label;
				// Use this for initialization
				void Start ()
				{
						label = GetComponent<Text> ();
		
						property = new LabelProperty (binding);
						property.AddListener (OnValueChange);
		
						OnValueChange (null);
				}
				void OnValueChange (System.Object value)
				{
						label.text = string.Format (format, property.value);
				}
				
		}
}
#endif