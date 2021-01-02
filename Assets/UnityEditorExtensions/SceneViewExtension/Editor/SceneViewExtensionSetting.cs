#if UNITY_EDITOR
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorExtensions
{
	public enum KeyBind
	{
		Always,
		Control,
		Alt,
		Shift,
	}

	public enum BoneType
	{
		All,
		Humanoid,
		HumanoidBodyOnly,
	}

	public class SceneViewExtensionSetting : ScriptableObject
	{
		// Serialize
		[Header("Default Options")]
		[SerializeField]
		KeyBind keyBind = KeyBind.Alt;
		[SerializeField]
		BoneType boneType = BoneType.All;
		[SerializeField, Range(0, 255)]
		int searchLimit = 0;

		[Header("Joint & Bone")]
		[SerializeField]
		Color jointColor = new Color(0f, 1f, 0f, 0.66f);
		[SerializeField]
		Color boneColor = new Color(0f, 1f, 0f);
		[SerializeField, Range(0.01f, 1.0f)]
		float extent = 0.02f;

		[Header("Label")]
		[SerializeField]
		bool hideLabel = false;
		[SerializeField, Range(8, 64)]
		int fontSize = 12;

		// Properties
		public KeyBind KeyBind => KeyBind;
		public BoneType BoneType => boneType;
		public int SearchLimit => searchLimit;
		public Color JointColor => jointColor;
		public Color BoneColor => boneColor;
		public float Extent => extent;
		public bool HideLabel => hideLabel;
		public int FontSize => fontSize;

		// Bone Table
		HumanBodyBones[] BodyBones => (boneType == BoneType.Humanoid ? FullBody : BodyOnly);
		// Bone List
		static readonly HumanBodyBones[] FullBody = System.Enum.GetValues(typeof(HumanBodyBones))
																	.Cast<HumanBodyBones>()
																	.Where(x => x != HumanBodyBones.LastBone)
																	.ToArray();
		static readonly HumanBodyBones[] BodyOnly = {
			HumanBodyBones.Hips,
			HumanBodyBones.LeftUpperLeg,
			HumanBodyBones.RightUpperLeg,
			HumanBodyBones.LeftLowerLeg,
			HumanBodyBones.RightLowerLeg,
			HumanBodyBones.LeftFoot,
			HumanBodyBones.RightFoot,
			HumanBodyBones.Spine,
			HumanBodyBones.Chest,
			HumanBodyBones.Neck,
			HumanBodyBones.Head,
			HumanBodyBones.LeftShoulder,
			HumanBodyBones.RightShoulder,
			HumanBodyBones.LeftUpperArm,
			HumanBodyBones.RightUpperArm,
			HumanBodyBones.LeftLowerArm,
			HumanBodyBones.RightLowerArm,
			HumanBodyBones.LeftHand,
			HumanBodyBones.RightHand,
			HumanBodyBones.LeftToes,
			HumanBodyBones.RightToes,
			HumanBodyBones.UpperChest,
		};


		/// <summary>
		/// Load Setting File.
		/// </summary>
		/// <returns></returns>
		public static SceneViewExtensionSetting Load()
		{
			var path = "Assets/UnityEditorExtensions/SceneViewExtension/SceneViewExtensionSetting.asset";
			var setting = AssetDatabase.LoadAssetAtPath<SceneViewExtensionSetting>(path);

			if (setting == null) {
				setting = ScriptableObject.CreateInstance<SceneViewExtensionSetting>();
				AssetDatabase.CreateAsset(setting, path);
			}

			return setting;
		}

		/// <summary>
		/// Valid Test.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public bool IsValidAll(Transform t) => (IsValidKeyBind() && IsValidDepth(t) && IsValidBone(t));

		public bool IsValidKeyBind()
		{
			switch (keyBind) {
			case KeyBind.Always:
				return true;
			case KeyBind.Control:
				return Event.current.control;
			case KeyBind.Alt:
				return Event.current.alt;
			case KeyBind.Shift:
				return Event.current.shift;
			}

			return false;
		}

		public bool IsValidDepth(Transform t)
		{
			if (searchLimit == 0) {
				return true;
			}

			var root = Selection.activeTransform;
			int depth = 0;

			if (t == root) {
				return true;
			}

			while (t.parent != null) {
				if (++depth > searchLimit) {
					return false;
				}

				t = t.parent;
				if (t == root) {
					return true;
				}
			}

			return false;
		}

		public bool IsValidBone(Transform t)
		{
			if (boneType == BoneType.All) {
				return true;
			}

			var animator = Selection.activeTransform?.GetComponentInParent<Animator>();

			if (animator == null) {
				return false;
			}

			var bones = BodyBones;

			for (int i = 0; i < bones.Length; i++) {
				var bone = animator.GetBoneTransform(bones[i]);

				if (bone == t) {
					return true;
				}
			}

			return false;
		}
	}
}
#endif
