#if UNITY_4_6
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Bindings
{

		public class ColorPropertyAttachment:MonoBehaviour
		{
				public string
						binding;
				public BindingDirection bindingDirection = BindingDirection.BindingToProperty;
				public AssignmentOnAdd assignmentOnAdd = AssignmentOnAdd.TakeBindingValue;
				private GraphicColorProperty  property;
				public GraphicColorProperty GetProperty ()
				{
						return property;
				}
				void Start ()
				{
						property = new GraphicColorProperty (GetComponent<Graphic> ());
						property.AddToBinding (binding, bindingDirection, assignmentOnAdd);
						
				}
		}
	
}
#endif

