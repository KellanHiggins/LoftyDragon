using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
namespace Bindings
{
		public enum AssignmentOnAdd
		{
				TakeBindingValue,
				TakePropertyValue,
				DoNothing
		}
		public delegate void StringChange (string value);

		public class BindingLabelUtils
		{
		
				public static void AddLabelBinding (string name, LabelProperty label)
				{
						if (labelPropertyDictionary.ContainsKey (name)) {
								Property<System.Object> property = GetLabelProperty (name);
								property.AddListener (label.SetValue);
								label.SetValue (property.value);
						}
				}
				
				public static void RemoveLabelBinding (string name, LabelProperty label)
				{
						if (labelPropertyDictionary.ContainsKey (name)) {
								GetLabelProperty (name).RemoveListener (label.SetValue);
						}
				}
				private static Dictionary<string,Property<System.Object>> labelPropertyDictionary = new Dictionary<string, Property<System.Object>> ();
				
				public static bool PropertyExists (string name)
				{
						return labelPropertyDictionary.ContainsKey (name);
				}
				protected static void AddLabelBinding (string nameOfBinding, BindingLabelProperty property)
				{
						if (!labelPropertyDictionary.ContainsKey (nameOfBinding)) {
								labelPropertyDictionary.Add (nameOfBinding, property);
						}
				}
				protected static void RemoveLabelBinding (string nameOfBinding)
				{
						if (labelPropertyDictionary.ContainsKey (nameOfBinding)) {
								labelPropertyDictionary.Remove (nameOfBinding);
						}
				}
				public static Property<System.Object> GetLabelProperty (string name)
				{
						if (labelPropertyDictionary.ContainsKey (name)) {
								return labelPropertyDictionary [name];
						}
						return null;
			
				}
		
				protected sealed class BindingLabelProperty:Property<System.Object>
				{
						public BindingLabelProperty (System.Object value)
						{
								_value = value;
			
			
						}
						public override void SetValue (System.Object value)
						{
								Set (value);
								NotifyChangeListeners (value);
						}
				}
		}
		public  class Binding<T>:BindingLabelUtils
//		where T:  System.IConvertible,System.IComparable<T>, System.IEquatable<T>
		{

				private static Dictionary<string,Binding<T>> bindingsDictionary = new Dictionary<string, Binding<T>> ();
				public static bool BindingExists (string name)
				{
						return bindingsDictionary.ContainsKey (name);
				}
				public static Binding<T> GetBinding (string name)
				{
						if (bindingsDictionary.ContainsKey (name)) {
								return bindingsDictionary [name];
						}
						return null;
					
				}
				public Binding (T value = default(T), string discoverableName=null)
				{
						BindingsCleaner.Add (CleanupBinding);
						_value = value;
						_type = typeof(T);
						_name = discoverableName;
						labelProperty = new BindingLabelProperty (value as System.Object);

						if (_name != null) {
								BindingLabelUtils.AddLabelBinding (_name, labelProperty);
								if (!bindingsDictionary.ContainsKey (_name)) {
										bindingsDictionary.Add (_name, this);
								} else {
										Debug.LogWarning ("Key {0} is already in use, assigning variable _name with null value and not adding it to bindingsDictionary");
										_name = null;
								}
							
						}
				}

				protected virtual void CleanupBinding ()
				{
						//Finalize ();
				}
				~Binding ()
				{

						if (bindingsDictionary.ContainsKey (_name)) {
								bindingsDictionary.Remove (_name);
						}
						propertyListeners = null;
						labelListeners = null;
						RemoveLabelBinding (_name);
			
			
				}
				public System.Type propertyType;

				public System.Type PropertyType {
						get {
								return propertyType;
						}

				}

				public void AddProperty (Property<T> property, BindingDirection direction, AssignmentOnAdd assignment)
				{
						if (_type == property.GetPropertyType ()) {
								if (assignment == AssignmentOnAdd.TakeBindingValue) {
										property.SetValue (_value);
				
								} else if (assignment == AssignmentOnAdd.TakePropertyValue) {
										OnNewValue (property.value);
								}
								
						}
						AddProperty (property, direction);
						
				}
				public void AddProperty (Property<T> property, BindingDirection direction)
				{
						BindingsCleaner.Check ();
						if (property.GetType () != typeof(LabelProperty)) {
//								Debug.Log (_type + "," + property.GetPropertyType ());
								if (_type == property.GetPropertyType ()) {
										
									
										if (direction == BindingDirection.BiDirectional || direction == BindingDirection.PropertyToBinding) {
//												property.AddBinding (this);
												property.AddListener (OnNewValue);
//												Debug.Log ("Bound ");
										}
				
										if (direction == BindingDirection.BiDirectional || direction == BindingDirection.BindingToProperty) {
												if (!propertyListeners.Contains (property))
														propertyListeners.Add (property);
								
										}
						
								
								}  
						}
				}
				public void AddLabelProperty (LabelProperty property)
				{
						if (!labelListeners.Contains (property)) {
								labelListeners.Add (property);
								
						}
				}
				public void RemoveProperty (Property<T> property)
				{
						if (property.GetType () != typeof(LabelProperty)) {
								
								if (_type == property.GetPropertyType ()) {
				
				
								
//										property.RemoveBinding (this);
										property.RemoveListener (OnNewValue);
										if (propertyListeners.Contains (property))
												propertyListeners.Remove (property);
				
				
								} 
						}
				}
				public void RemoveLabelProperty (LabelProperty property)
				{
						if (labelListeners.Contains (property))
								labelListeners.Remove (property);
				}

				protected List<Property<T>> propertyListeners = new List<Property<T>> ();
				protected List<LabelProperty> labelListeners = new List<LabelProperty> ();
				protected System.Type _type;
				protected readonly string _name;
				
				public string GetName ()
				{
						return _name;
				}

				protected T _value;
				public T value ()
				{
						return _value;
				}
				protected BindingLabelProperty labelProperty;
				public void OnNewValue (T value)
				{
						if (!value.Equals (_value)) {
								_value = value;
								
								labelProperty.SetValue (value as System.Object);
								foreach (Property<T> p in propertyListeners) {
										p.SetValue (value);
								}
								
						}
				}

				
		}
}