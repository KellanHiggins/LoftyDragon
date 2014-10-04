using UnityEngine;
using System.Collections;
using Bindings;
namespace Bindings
{
		public abstract class LerpBetween : MonoBehaviour
		{

				[SerializeField]
				private Transform
						position1;
				[SerializeField]
				private Transform
						position2;
				[SerializeField]
				private string
						positionBinding;
				Property<float> positionProperty;
				protected abstract float GetMaxValue ();
				protected void Start ()
				{
						positionProperty = new Property<float> (0);
						positionProperty.AddToBinding (positionBinding, Bindings.BindingDirection.BindingToProperty, Bindings.AssignmentOnAdd.TakeBindingValue);
						positionProperty.AddListener (OnNewValue);
						OnNewValue (0);
				}
				protected virtual void OnNewValue (float value)
				{
						transform.position = Vector3.Lerp (position1.position, position2.position, positionProperty.value / GetMaxValue ());
				}
		}
}