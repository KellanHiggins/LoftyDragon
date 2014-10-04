using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bindings
{
		public enum BindingDirection
		{
				BiDirectional,
				PropertyToBinding,
				BindingToProperty
		}
		public class Property<T>
		{

//				string binding = 
				protected List<Binding<T>> bindingListners = new List<Binding<T>> ();
				public Property (T value)
				{
						BindingsCleaner.Add (CleanupProperty);
						_value = value;
						_type = typeof(T);

						
				}
				protected Property ()
				{
						BindingsCleaner.Add (CleanupProperty);
				}


				protected virtual void CleanupProperty ()
				{
						//Finalize ();
				}
				~Property ()
				{

			
			
				}
				public void AddToBinding (string bindingName, BindingDirection direction, AssignmentOnAdd assignmentIfExists)
				{
						if (Binding<T>.BindingExists (bindingName)) {
								Binding<T>.GetBinding (bindingName).AddProperty (this, direction, assignmentIfExists);
						} else {
								Binding<T> newBinding = new Binding<T> (_value, bindingName);
								newBinding.AddProperty (this, direction);
						}
				}
				public void RemoveFromBinding (string bindingName)
				{
						if (Binding<T>.BindingExists (bindingName)) {
								Binding<T>.GetBinding (bindingName).RemoveProperty (this);
						}
				}

				protected System.Type _type;
				protected T _value;
				public T value 
				{ get { return _value; } set {

								SetValue (value);
								
						} }
				public virtual void SetValue (T value)
				{
						bool updateBindings = true;
						if (!value.Equals (_value)) {

								Set (value);
								
								NotifyChangeListeners (value);
						} else if (updateBindings) {
								NotifyChangeListeners (value);
						}
				}
				protected virtual void Set (T value)
				{
						this._value = value;
				}


				public System.Type GetPropertyType ()
				{
						return _type;
				}
				protected List<OnValueChangeEvent> OnChangeListeners = new List<OnValueChangeEvent> ();
				public void AddListener (OnValueChangeEvent evt)
				{
						if (!OnChangeListeners.Contains (evt)) {
								OnChangeListeners.Add (evt);
						} else {
								Debug.Log ("Duplicate Delegate");
						}
				}
				public void RemoveListener (OnValueChangeEvent evt)
				{
						if (OnChangeListeners.Contains (evt))
								OnChangeListeners.Remove (evt);
				}
				protected void NotifyChangeListeners (T value)
				{
						foreach (OnValueChangeEvent evt in OnChangeListeners) {
								evt (value);
						}
				}
				public delegate void OnValueChangeEvent (T value);
		}
}