#if UNITY_4_6
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Bindings;
[RequireComponent(typeof(Image))]
public abstract class FillPropertyBaseClass : MonoBehaviour
{
		[SerializeField]
		protected string
				binding;

		protected abstract float GetMaxValue ();

		protected ImageFillProperty  currentValueProperty;
		public ImageFillProperty GetProperty ()
		{
				return currentValueProperty;
		}
		protected void Start ()
		{
				currentValueProperty = new ImageFillProperty (GetComponent<Image> (), GetMaxValue ());
				currentValueProperty.AddToBinding (binding, Bindings.BindingDirection.BindingToProperty, Bindings.AssignmentOnAdd.TakeBindingValue);
		
		}

}
#endif