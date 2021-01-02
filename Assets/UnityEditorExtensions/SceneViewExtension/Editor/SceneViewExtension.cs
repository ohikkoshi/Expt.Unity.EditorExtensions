#if UNITY_EDITOR
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorExtensions
{
	public class SceneViewExtension
	{
		static SceneViewExtensionSetting setting;


		[DrawGizmo(GizmoType.Selected)]
		static void OnGizmoSelected(Transform t, GizmoType type)
		{
			if (setting == null) {
				setting = SceneViewExtensionSetting.Load();
			}

			if (SceneView.currentDrawingSceneView != null) {
				OnSearchGizmos(t);
			}
		}

		static void OnSearchGizmos(Transform t)
		{
			OnDrawGizmos(t);

			for (int i = 0; i < t.childCount; i++) {
				var c = t.GetChild(i);
				OnSearchGizmos(c);
			}
		}

		static void OnDrawGizmos(Transform t)
		{
			if (!setting.IsValidAll(t)) {
				return;
			}

			float Extent = setting.Extent;
			float HalfExtent = Extent / 2f;

			// Joint
			Handles.color = setting.JointColor;
			Handles.SphereHandleCap(0, t.position, t.rotation, Extent, Event.current.type);
			Handles.ScaleHandle(Vector3.one, t.position, t.rotation, Extent);

			// Bone
			Handles.color = setting.BoneColor;
			var V0 = new Vector3(0f, 0f, -HalfExtent);
			var V1 = new Vector3(-HalfExtent, HalfExtent, HalfExtent);
			var V2 = new Vector3(HalfExtent, HalfExtent, HalfExtent);
			var V3 = new Vector3(HalfExtent, -HalfExtent, HalfExtent);
			var V4 = new Vector3(-HalfExtent, -HalfExtent, HalfExtent);

			for (int i = 0; i < t.childCount; i++) {
				var c = t.GetChild(i);

				if (!setting.IsValidBone(c)) {
					continue;
				}

				var v = c.position - t.position;

				if (v == Vector3.zero) {
					continue;
				}

				var look = Quaternion.LookRotation(v);
				var vertices = new Vector3[]{
					c.position + look * V0,
					t.position + look * V1,
					t.position + look * V2,
					t.position + look * V3,
					t.position + look * V4,
				};
				var segments = new Vector3[]{
					vertices[0], vertices[1],
					vertices[0], vertices[2],
					vertices[0], vertices[3],
					vertices[0], vertices[4],
					vertices[1], vertices[2],
					vertices[2], vertices[3],
					vertices[3], vertices[4],
					vertices[4], vertices[1],
				};

				Handles.DrawLines(segments);
			}

			if (setting.HideLabel) {
				return;
			}

			// Label
			Handles.BeginGUI();
			{
				string label = t.name;

				// font calc
				GUI.skin.button.fontSize = setting.FontSize;
				GUI.skin.button.alignment = TextAnchor.MiddleCenter;
				var size = GUI.skin.button.CalcSize(new GUIContent(label));

				// screen calc
				var view = SceneView.currentDrawingSceneView;
				var scrn = view.camera.WorldToScreenPoint(t.position);

				if (scrn.z <= 0f) {
					return;
				}

				// retina display
				scrn *= (1f / EditorGUIUtility.pixelsPerPoint);

				var rect = new Rect(
						scrn.x - (size.x * 0.5f),
						view.position.height - scrn.y - (size.y * 0.5f),
						size.x,
						size.y
						);
				GUI.Button(rect, label);

				//if (rect.Contains(Event.current.mousePosition)) {
				//}
			}
			Handles.EndGUI();
		}
	}
}
#endif
