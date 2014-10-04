#if UNITY_4_6
using UnityEngine;
using System.Collections;
using Bindings;
public class FillPropertyAttachmentBindingEndpoint : FillPropertyBaseClass
{
		[SerializeField]
		protected string
				maxValueBinding;
		Property<float> maxValueProperty;
		
		protected override float GetMaxValue ()
		{
				if (maxValueProperty == null) {
						maxValueProperty = new Property<float> (0);
						maxValueProperty.AddToBinding (maxValueBinding, BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
				}
				return maxValueProperty.value;
		}

}
#endif