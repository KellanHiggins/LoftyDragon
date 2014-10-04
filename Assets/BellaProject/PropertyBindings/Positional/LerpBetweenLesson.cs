using UnityEngine;
using System.Collections;
using Bindings;
public class LerpBetweenLesson : MonoBehaviour
{

		[SerializeField]
		private Transform
				position1;
		[SerializeField]
		private Transform
				position2;
		Property<float> maxValue;
		Property<float> positionValue;
		[SerializeField]
		private string
				maxValueBinding;
		[SerializeField]
		private string
				positionValueBinding;
		
		// Use this for initialization
		void Start ()
		{
				maxValue = new Property<float> (0);
				maxValue.AddToBinding (maxValueBinding, BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
				maxValue.AddListener (OnChange);
				positionValue = new Property<float> (0);
				positionValue.AddToBinding (positionValueBinding, BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
				positionValue.AddListener (OnChange);
				
				OnChange (0);
		}
		void OnChange (float value)
		{
				transform.position = Vector3.Lerp (position1.position, position2.position, (positionValue.value / maxValue.value));
		}
	

}
