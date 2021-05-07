using UnityEngine;
using UnityEditor;

namespace GameApp.BezierCurves
{
	[CustomEditor(typeof(BezierPoint))]
	[CanEditMultipleObjects]
	public class BezierPointEditor : Editor
	{
		private BezierPoint _point;

		private SerializedProperty _handleTypeProp;
		private SerializedProperty _handle1Prop;
		private SerializedProperty _handle2Prop;

		private delegate void HandleFunction(BezierPoint p);

		private readonly HandleFunction[] _handlers = {HandleConnected, HandleBroken, HandleAbsent};

		private void OnEnable()
		{
			_point = (BezierPoint) target;

			_handleTypeProp = serializedObject.FindProperty("handleStyle");
			_handle1Prop = serializedObject.FindProperty("_handle1");
			_handle2Prop = serializedObject.FindProperty("_handle2");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var newType = (BezierPoint.HandleStyle) EditorGUILayout.EnumPopup("Handle Type",
				(BezierPoint.HandleStyle) _handleTypeProp.intValue);

			if (newType != (BezierPoint.HandleStyle) _handleTypeProp.intValue)
			{
				_handleTypeProp.intValue = (int) newType;

				if (newType == BezierPoint.HandleStyle.Connected)
				{
					if (_handle1Prop.vector3Value != Vector3.zero)
						_handle2Prop.vector3Value = -_handle1Prop.vector3Value;
					else if (_handle2Prop.vector3Value != Vector3.zero)
						_handle1Prop.vector3Value = -_handle2Prop.vector3Value;
					else
					{
						_handle1Prop.vector3Value = new Vector3(0.1f, 0, 0);
						_handle2Prop.vector3Value = new Vector3(-0.1f, 0, 0);
					}
				}

				else if (newType == BezierPoint.HandleStyle.Broken)
				{
					if (_handle1Prop.vector3Value == Vector3.zero && _handle2Prop.vector3Value == Vector3.zero)
					{
						_handle1Prop.vector3Value = new Vector3(0.1f, 0, 0);
						_handle2Prop.vector3Value = new Vector3(-0.1f, 0, 0);
					}
				}
				else if (newType == BezierPoint.HandleStyle.None)
				{
					_handle1Prop.vector3Value = Vector3.zero;
					_handle2Prop.vector3Value = Vector3.zero;
				}
			}

			if (newType != BezierPoint.HandleStyle.None)
			{
				var newHandle1 = EditorGUILayout.Vector3Field("Handle 1", _handle1Prop.vector3Value);
				var newHandle2 = EditorGUILayout.Vector3Field("Handle 2", _handle2Prop.vector3Value);

				if (newType == BezierPoint.HandleStyle.Connected)
				{
					if (newHandle1 != _handle1Prop.vector3Value)
					{
						_handle1Prop.vector3Value = newHandle1;
						_handle2Prop.vector3Value = -newHandle1;
					}
					else if (newHandle2 != _handle2Prop.vector3Value)
					{
						_handle1Prop.vector3Value = -newHandle2;
						_handle2Prop.vector3Value = newHandle2;
					}
				}
				else
				{
					_handle1Prop.vector3Value = newHandle1;
					_handle2Prop.vector3Value = newHandle2;
				}
			}

			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(target);
			}
		}

		private void OnSceneGUI()
		{
			Handles.color = Color.green;
			var newPosition = Handles.FreeMoveHandle(_point.position, _point.transform.rotation,
				HandleUtility.GetHandleSize(_point.position) * 0.2f,
				Vector3.zero, Handles.CubeHandleCap);
			if (_point.position != newPosition) _point.position = newPosition;

			_handlers[(int) _point.handleStyle](_point);

			Handles.color = Color.yellow;
			Handles.DrawLine(_point.position, _point.globalHandle1);
			Handles.DrawLine(_point.position, _point.globalHandle2);

			BezierCurveEditor.DrawOtherPoints(_point.curve, _point);
		}

		private static void HandleConnected(BezierPoint p)
		{
			Handles.color = Color.cyan;

			var newGlobal1 = Handles.FreeMoveHandle(
				p.globalHandle1, p.transform.rotation,
				HandleUtility.GetHandleSize(p.globalHandle1) * 0.15f,
				Vector3.zero, Handles.SphereHandleCap);

			if (newGlobal1 != p.globalHandle1)
			{
				Undo.RecordObject(p, "Move Handle");
				p.globalHandle1 = newGlobal1;
				p.globalHandle2 = -(newGlobal1 - p.position) + p.position;
			}

			var newGlobal2 = Handles.FreeMoveHandle(
				p.globalHandle2, p.transform.rotation,
				HandleUtility.GetHandleSize(p.globalHandle2) * 0.15f,
				Vector3.zero, Handles.SphereHandleCap);

			if (newGlobal2 != p.globalHandle2)
			{
				Undo.RecordObject(p, "Move Handle");
				p.globalHandle1 = -(newGlobal2 - p.position) + p.position;
				p.globalHandle2 = newGlobal2;
			}
		}

		private static void HandleBroken(BezierPoint p)
		{
			Handles.color = Color.cyan;

			var newGlobal1 = Handles.FreeMoveHandle(
				p.globalHandle1, Quaternion.identity,
				HandleUtility.GetHandleSize(p.globalHandle1) * 0.15f,
				Vector3.zero, Handles.SphereHandleCap);

			var newGlobal2 = Handles.FreeMoveHandle(
				p.globalHandle2, Quaternion.identity,
				HandleUtility.GetHandleSize(p.globalHandle2) * 0.15f,
				Vector3.zero, Handles.SphereHandleCap);

			if (newGlobal1 != p.globalHandle1)
			{
				Undo.RecordObject(p, "Move Handle");
				p.globalHandle1 = newGlobal1;
			}

			if (newGlobal2 != p.globalHandle2)
			{
				Undo.RecordObject(p, "Move Handle");
				p.globalHandle2 = newGlobal2;
			}
		}

		private static void HandleAbsent(BezierPoint p)
		{
			p.handle1 = Vector3.zero;
			p.handle2 = Vector3.zero;
		}
	}
}