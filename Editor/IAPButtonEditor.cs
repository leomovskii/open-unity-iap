#if UNITY_EDITOR

using OpenIAP;
using UnityEditor;

namespace OpenIAPEditor {
	[CustomEditor(typeof(IAPButton))]
	[CanEditMultipleObjects]
	public class IAPButtonEditor : AbstractIAPButtonEditor {

		public void OnEnable() {
			OnEnableInternal();
		}

		public override void OnInspectorGUI() {
			OnInspectorGuiInternal();
		}
	}
}

#endif