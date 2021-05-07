using UnityEngine;
using UnityEditor;

namespace GameApp.BezierCurves
{
	[CustomEditor(typeof(BezierCurve))]
	public class BezierCurveEditor : Editor
	{
		private BezierCurve _curve;
		private SerializedProperty _resolutionProp;
		private SerializedProperty _closeProp;
		private SerializedProperty _pointsProp;
		private SerializedProperty _colorProp;

		private static bool _showPoints = true;

		private void OnEnable()
		{
			_curve = (BezierCurve) target;

			_resolutionProp = serializedObject.FindProperty("resolution");
			_closeProp = serializedObject.FindProperty("_close");
			_pointsProp = serializedObject.FindProperty("points");
			_colorProp = serializedObject.FindProperty("drawColor");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_resolutionProp);
			EditorGUILayout.PropertyField(_closeProp);
			EditorGUILayout.PropertyField(_colorProp);

			_showPoints = EditorGUILayout.Foldout(_showPoints, "Points");

			if (_showPoints)
			{
				var pointCount = _pointsProp.arraySize;

				for (var i = 0; i < pointCount; i++)
				{
					DrawPointInspector(_curve[i], i);
				}

				if (GUILayout.Button("Add Point"))
				{
					//Undo.RegisterSceneUndo("Add Point");

					var pointObject = new GameObject("Point " + _pointsProp.arraySize);
					pointObject.transform.parent = _curve.transform;
					pointObject.transform.localPosition = Vector3.zero;
					var newPoint = pointObject.AddComponent<BezierPoint>();

					newPoint.curve = _curve;
					newPoint.handle1 = Vector3.right * 0.1f;
					newPoint.handle2 = -Vector3.right * 0.1f;

					_pointsProp.InsertArrayElementAtIndex(_pointsProp.arraySize);
					_pointsProp.GetArrayElementAtIndex(_pointsProp.arraySize - 1).objectReferenceValue = newPoint;
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
			for (var i = 0; i < _curve.pointCount; i++)
			{
				DrawPointSceneGui(_curve[i]);
			}
		}

		private void DrawPointInspector(BezierPoint point, int index)
		{
			var serObj = new SerializedObject(point);

			var handleStyleProp = serObj.FindProperty("handleStyle");
			var handle1Prop = serObj.FindProperty("_handle1");
			var handle2Prop = serObj.FindProperty("_handle2");

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("X", GUILayout.Width(20)))
			{
				//Undo.RegisterUndo("Remove Point");
				_pointsProp.MoveArrayElement(_curve.GetPointIndex(point), _curve.pointCount - 1);
				_pointsProp.arraySize--;
				DestroyImmediate(point.gameObject);
				return;
			}

			EditorGUILayout.ObjectField(point.gameObject, typeof(GameObject), true);

			if (index != 0 && GUILayout.Button(@"/\", GUILayout.Width(25)))
			{
				var other = _pointsProp.GetArrayElementAtIndex(index - 1).objectReferenceValue;
				_pointsProp.GetArrayElementAtIndex(index - 1).objectReferenceValue = point;
				_pointsProp.GetArrayElementAtIndex(index).objectReferenceValue = other;
			}

			if (index != _pointsProp.arraySize - 1 && GUILayout.Button(@"\/", GUILayout.Width(25)))
			{
				var other = _pointsProp.GetArrayElementAtIndex(index + 1).objectReferenceValue;
				_pointsProp.GetArrayElementAtIndex(index + 1).objectReferenceValue = point;
				_pointsProp.GetArrayElementAtIndex(index).objectReferenceValue = other;
			}

			EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel++;
			EditorGUI.indentLevel++;

			var newType = (BezierPoint.HandleStyle) EditorGUILayout.EnumPopup("Handle Type",
				(BezierPoint.HandleStyle) handleStyleProp.intValue);

			if (newType != (BezierPoint.HandleStyle) handleStyleProp.intValue)
			{
				handleStyleProp.intValue = (int) newType;
				if (newType == BezierPoint.HandleStyle.Connected)
				{
					if (handle1Prop.vector3Value != Vector3.zero) handle2Prop.vector3Value = -handle1Prop.vector3Value;
					else if (handle2Prop.vector3Value != Vector3.zero)
						handle1Prop.vector3Value = -handle2Prop.vector3Value;
					else
					{
						handle1Prop.vector3Value = new Vector3(0.1f, 0, 0);
						handle2Prop.vector3Value = new Vector3(-0.1f, 0, 0);
					}
				}

				else if (newType == BezierPoint.HandleStyle.Broken)
				{
					if (handle1Prop.vector3Value == Vector3.zero && handle2Prop.vector3Value == Vector3.zero)
					{
						handle1Prop.vector3Value = new Vector3(0.1f, 0, 0);
						handle2Prop.vector3Value = new Vector3(-0.1f, 0, 0);
					}
				}

				else if (newType == BezierPoint.HandleStyle.None)
				{
					handle1Prop.vector3Value = Vector3.zero;
					handle2Prop.vector3Value = Vector3.zero;
				}
			}

			var newPointPos = EditorGUILayout.Vector3Field("Position : ", point.transform.localPosition);
			if (newPointPos != point.transform.localPosition)
			{
				Undo.RecordObject(point.transform, "Move Bezier Point");
				point.transform.localPosition = newPointPos;
			}

			if (newType == BezierPoint.HandleStyle.Connected)
			{
				var newPosition = EditorGUILayout.Vector3Field("Handle 1", handle1Prop.vector3Value);
				if (newPosition != handle1Prop.vector3Value)
				{
					handle1Prop.vector3Value = newPosition;
					handle2Prop.vector3Value = -newPosition;
				}

				newPosition = EditorGUILayout.Vector3Field("Handle 2", handle2Prop.vector3Value);
				if (newPosition != handle2Prop.vector3Value)
				{
					handle1Prop.vector3Value = -newPosition;
					handle2Prop.vector3Value = newPosition;
				}
			}

			else if (newType == BezierPoint.HandleStyle.Broken)
			{
				EditorGUILayout.PropertyField(handle1Prop);
				EditorGUILayout.PropertyField(handle2Prop);
			}

			EditorGUI.indentLevel--;
			EditorGUI.indentLevel--;

			if (GUI.changed)
			{
				serObj.ApplyModifiedProperties();
				EditorUtility.SetDirty(serObj.targetObject);
			}
		}

		private static void DrawPointSceneGui(BezierPoint point)
		{
			Handles.Label(point.position + new Vector3(0, HandleUtility.GetHandleSize(point.position) * 0.4f, 0),
				point.gameObject.name);

			Handles.color = Color.green;
			var newPosition = Handles.FreeMoveHandle(point.position, point.transform.rotation,
				HandleUtility.GetHandleSize(point.position) * 0.1f,
				Vector3.zero, Handles.RectangleHandleCap);

			if (newPosition != point.position)
			{
				Undo.RecordObject(point.transform, "Move Point");
				point.transform.position = newPosition;
			}

			if (point.handleStyle != BezierPoint.HandleStyle.None)
			{
				Handles.color = Color.cyan;
				var newGlobal1 = Handles.FreeMoveHandle(
					point.globalHandle1, point.transform.rotation,
					HandleUtility.GetHandleSize(point.globalHandle1) * 0.075f,
					Vector3.zero, Handles.CircleHandleCap);
				if (point.globalHandle1 != newGlobal1)
				{
					Undo.RecordObject(point, "Move Handle");
					point.globalHandle1 = newGlobal1;
					if (point.handleStyle == BezierPoint.HandleStyle.Connected)
						point.globalHandle2 = -(newGlobal1 - point.position) + point.position;
				}

				var newGlobal2 = Handles.FreeMoveHandle(
					point.globalHandle2, point.transform.rotation,
					HandleUtility.GetHandleSize(point.globalHandle2) * 0.075f,
					Vector3.zero, Handles.CircleHandleCap);
				if (point.globalHandle2 != newGlobal2)
				{
					Undo.RecordObject(point, "Move Handle");
					point.globalHandle2 = newGlobal2;
					if (point.handleStyle == BezierPoint.HandleStyle.Connected)
						point.globalHandle1 = -(newGlobal2 - point.position) + point.position;
				}

				Handles.color = Color.yellow;
				Handles.DrawLine(point.position, point.globalHandle1);
				Handles.DrawLine(point.position, point.globalHandle2);
			}
		}

		public static void DrawOtherPoints(BezierCurve curve, BezierPoint caller)
		{
			foreach (var p in curve.GetAnchorPoints())
			{
				if (p != caller) DrawPointSceneGui(p);
			}
		}

		[MenuItem("GameObject/Create Other/Bezier Curve")]
		public static void CreateCurve(MenuCommand command)
		{
			var curveObject = new GameObject("BezierCurve");
			Undo.RecordObject(curveObject, "Undo Create Curve");
			var curve = curveObject.AddComponent<BezierCurve>();

			var p1 = curve.AddPointAt(Vector3.forward * 0.5f);
			p1.handleStyle = BezierPoint.HandleStyle.Connected;
			p1.handle1 = new Vector3(-0.28f, 0, 0);

			var p2 = curve.AddPointAt(Vector3.right * 0.5f);
			p2.handleStyle = BezierPoint.HandleStyle.Connected;
			p2.handle1 = new Vector3(0, 0, 0.28f);

			var p3 = curve.AddPointAt(-Vector3.forward * 0.5f);
			p3.handleStyle = BezierPoint.HandleStyle.Connected;
			p3.handle1 = new Vector3(0.28f, 0, 0);

			var p4 = curve.AddPointAt(-Vector3.right * 0.5f);
			p4.handleStyle = BezierPoint.HandleStyle.Connected;
			p4.handle1 = new Vector3(0, 0, -0.28f);

			curve.close = true;
		}
	}
}