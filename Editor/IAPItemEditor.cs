#if UNITY_EDITOR

using OpenIAP;
using UnityEditor;

namespace OpenIAPEditor {
	[CustomEditor(typeof(IAPItem))]
	[CanEditMultipleObjects]
	public class IAPItemEditor : AbstractIAPItemEditor {

		public void OnEnable() {
			OnEnableInternal();
		}

		public override void OnInspectorGUI() {
			OnInspectorGuiInternal();
		}
	}
}

#endif