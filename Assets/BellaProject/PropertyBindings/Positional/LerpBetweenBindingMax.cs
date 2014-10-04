using UnityEngine;
using System.Collections;
using Bindings;
public class LerpBetweenBindingMax : LerpBetween
{

		[SerializeField]
		private string
				maxValueBinding;
		Property<float> maxValueProperty;
		protected override float GetMaxValue ()
		{
				if (maxValueProperty == null) {
						maxValueProperty = new Property<float> (0);
						maxValueProperty.AddToBinding (maxValueBinding, BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
						maxValueProperty.AddListener (OnNewValue);
				}
				return maxValueProperty.value;
		}
}
