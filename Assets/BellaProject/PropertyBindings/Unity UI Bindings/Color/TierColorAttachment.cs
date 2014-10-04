#if UNITY_4_6
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Bindings;

public class TierColorAttachment : MonoBehaviour
{
		[SerializeField]
		Color
				tooHigh;
		[SerializeField]
		Color
				good;
		[SerializeField]
		Color
				tooLow;
		[SerializeField]
		private string
				bindingForCurrentValue;
		[SerializeField]
		private string
				bindingForMinValue;
		[SerializeField]
		private string
				bindingForMaxValue;
		Property<float> propertyForCurrentValue;
		Property<float> propertyForMinValue;
		Property<float> propertyForMaxValue;
		Graphic graphic;
		// Use this for initialization
		void Start ()
		{
				graphic = GetComponent<Graphic> ();
				propertyForCurrentValue = new Property<float> (0);
				propertyForCurrentValue.AddToBinding (bindingForCurrentValue, BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
				propertyForMinValue = new Property<float> (0);
				propertyForMinValue.AddToBinding (bindingForMinValue, BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
				propertyForMaxValue = new Property<float> (0);
				propertyForMaxValue.AddToBinding (bindingForMaxValue, BindingDirection.BindingToProperty, AssignmentOnAdd.TakeBindingValue);
				
				
				propertyForCurrentValue.AddListener (NewValue);
		}
	
		// Update is called once per frame
		void NewValue (float value)
		{
				if (value > propertyForMaxValue.value) {
						graphic.color = Color.Lerp (graphic.color, tooHigh, 0.2f);
				} else if (value >= propertyForMinValue.value) {
						graphic.color = Color.Lerp (graphic.color, good, 0.2f);
				} else {
						graphic.color = Color.Lerp (graphic.color, tooLow, 0.2f);
				}
		}
}
#endif
