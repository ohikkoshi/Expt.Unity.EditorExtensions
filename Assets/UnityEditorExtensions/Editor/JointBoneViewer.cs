#if UNITY_EDITOR
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorUtilities
{
	public class JointBoneViewer
	{
		// Color
		static readonly Color JointColor = new Color(0f, 1f, 0f, 0.66f);
		static readonly Color BoneColor = new Color(0f, 1f, 0f);
		// Size
		const float Extent = 0.1f;
		const float HalfExtent = 0.05f;
		// Label
		static readonly Vector2 LabelSize = new Vector2(100f, 20f);


		[DrawGizmo(GizmoType.Selected)]
		static void OnGizmoSelected(Transform t, GizmoType type)
		{
			OnSearchGizmos(t);
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
			// Joint
			Handles.color = JointColor;
			Handles.SphereHandleCap(0, t.position, t.rotation, Extent, Event.current.type);
			Handles.ScaleHandle(Vector3.one, t.position, t.rotation, Extent);

			// Bone
			Handles.color = BoneColor;
			var V0 = new Vector3(0f, 0f, -HalfExtent);
			var V1 = new Vector3(-HalfExtent, HalfExtent, HalfExtent);
			var V2 = new Vector3(HalfExtent, HalfExtent, HalfExtent);
			var V3 = new Vector3(HalfExtent, -HalfExtent, HalfExtent);
			var V4 = new Vector3(-HalfExtent, -HalfExtent, HalfExtent);

			for (int i = 0; i < t.childCount; i++) {
				var c = t.GetChild(i);
				var v = c.position - t.position;
				if (v.magnitude == 0f) {
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

			// Label
			Handles.BeginGUI();
			{
				var view = SceneView.currentDrawingSceneView;

				if (view != null) {
					var scrn = view.camera.WorldToScreenPoint(t.position);
					var rect = new Rect(
							scrn.x - (LabelSize.x * 0.5f),
							view.position.height - scrn.y - (LabelSize.y * 0.5f),
							LabelSize.x,
							LabelSize.y
							);

					GUI.Button(rect, t.name);
				} else {
#if GameViewShowLabels
					var scrn = Camera.main.WorldToScreenPoint(t.position);
					var rect = new Rect(
							scrn.x - (LabelSize.x * 0.5f),
							Screen.height - scrn.y - (LabelSize.y * 0.5f),
							LabelSize.x,
							LabelSize.y
							);

					GUI.Button(rect, t.name);
#endif
				}
			}
			Handles.EndGUI();
		}
	}
}
#endif
