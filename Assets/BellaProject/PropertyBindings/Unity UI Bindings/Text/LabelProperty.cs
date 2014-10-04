using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Bindings
{
		public class LabelProperty : Property<System.Object>
		{

				string _name;
				public LabelProperty (string name, string formatType="")
				{
						_type = typeof(System.Object);
						_value = null;

						ListenToBinding (name);
				}

				public void ListenToBinding (string name)
				{
						
						if (_name != null) {
								StopListeningToBinding ();
						}
						_name = name;
						BindingLabelUtils.AddLabelBinding (name, this);
				}
				public void StopListeningToBinding ()
				{
						BindingLabelUtils.RemoveLabelBinding (_name, this);
						_name = null;
						
				}
				public override void SetValue (System.Object value)
				{
						if (_value == null && value == null) {
								return;
						}
						if (_value != null && value == null) {
								Set (null);
								NotifyChangeListeners (null);
								return;
						} else if (_value == null && value != null) {
								Set (value);
								NotifyChangeListeners (value);
								return;
						}
						base.SetValue (value);
				}
				

		}

}