using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;
using PM.UsefulThings.SimpleJson;

namespace PM.UsefulThings.Extensions
{
	public static class UnityExtensions
	{
		#region GameObject
		public static T FindInParents<T>(this GameObject go) where T : Component
		{
			if (go == null)
				return null;

			T comp = go.GetComponent<T>();

			if (comp == null)
			{
				Transform t = go.transform.parent;

				while (t != null && comp == null)
				{
					comp = t.gameObject.GetComponent<T>();
					t = t.parent;
				}
			}

			return comp;
		}

		public static T GetOrAddComponent<T>(this GameObject go) where T : Component
		{
			if (go == null)
				return null;

			T comp = go.GetComponent<T>();

			if (comp == null)
				comp = go.AddComponent<T>();

			return comp;
		}

		public static GameObject FindChildWithName(this GameObject go, string name)
		{
			if (go == null)
				return null;

			if (go.name == name)
				return go;

			foreach (var child in go.GetComponentsInChildren<Transform>(true))
			{
				if (child.name == name)
				{
					return child.gameObject;
				}
			}
			return null;
		}

		public static List<GameObject> GetAllChildsRecursive(this GameObject go)
		{
			if (go == null)
			{
				return null;
			}

			var listOfChilds = new List<GameObject>();
			foreach (Transform tr in go.transform)
			{
				listOfChilds.Add(tr.gameObject);
				listOfChilds.AddRange(GetAllChildsRecursive(tr.gameObject));
			}

			return listOfChilds;
		}

		public static T GetExistingComponent<T>(this Component _this) where T : Component
		{
			return _this.gameObject.GetExistingComponent<T>();
		}

		public static T GetExistingComponent<T>(this GameObject _this) where T : Component
		{
			var c = _this.GetComponent<T>();

			if (c == null)
			{
				Debug.LogError(_this.name + ": required component not found: " + typeof(T).Name);
			}

			return c;
		}

		public static T GetComponent<T>(this Component _this, string childName) where T : Component
		{
			return _this.gameObject.GetComponent<T>(childName);
		}

		public static T GetComponent<T>(this GameObject _this, string childName) where T : Component
		{
			var obj = _this.transform.Find(childName);
			return obj != null ? obj.GetComponent<T>() : null;
		}

		public static T GetExistingComponent<T>(this Component _this, string childName) where T : Component
		{
			return _this.gameObject.GetExistingComponent<T>(childName);
		}

		public static T GetExistingComponent<T>(this GameObject _this, string childName) where T : Component
		{
			var obj = _this.transform.Find(childName);
			var c = obj != null ? obj.GetComponent<T>() : null;

			if (c == null)
			{
				Debug.LogError(_this.name + ": required component or child object not found: '" + childName + "'." + typeof(T).Name);
			}

			return c;
		}

		public static void SetLayerRecursively(this GameObject go, int layer)
		{
			go.layer = layer;

			foreach (Transform child in go.transform)
			{
				child.gameObject.SetLayerRecursively(layer);
			}
		}

		public static void ResetPRS(this Transform obj)
		{
			obj.localPosition = Vector3.zero;
			obj.localRotation = Quaternion.identity;
			obj.localScale = Vector3.one;
		}

		public static void ResetPRS(this GameObject obj)
		{
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;
		}

		public static void ResetPRS(this Component obj)
		{
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;
		}

		public static void AddChildResetPRS(this Transform obj, GameObject child)
		{
			child.transform.parent = obj;
			child.transform.localPosition = Vector3.zero;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
		}

		public static void AddChildResetPRS(this GameObject obj, GameObject child)
		{
			child.transform.parent = obj.transform;
			child.transform.localPosition = Vector3.zero;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
		}

		public static void AddChildResetPRS(this Transform obj, Component child)
		{
			child.transform.parent = obj;
			child.transform.localPosition = Vector3.zero;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
		}

		public static void AddChildResetPRS(this GameObject obj, Component child)
		{
			child.transform.parent = obj.transform;
			child.transform.localPosition = Vector3.zero;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
		}

		public static void AddChildResetPRS(this Transform obj, GameObject child, Vector3 localPosition)
		{
			child.transform.parent = obj;
			child.transform.localPosition = localPosition;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
		}

		public static void AddChildResetPRS(this GameObject obj, GameObject child, Vector3 localPosition)
		{
			child.transform.parent = obj.transform;
			child.transform.localPosition = localPosition;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
		}

		public static void AddChildResetPRS(this Transform obj, Component child, Vector3 localPosition)
		{
			child.transform.parent = obj;
			child.transform.localPosition = localPosition;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
		}

		public static void AddChildResetPRS(this GameObject obj, Component child, Vector3 localPosition)
		{
			child.transform.parent = obj.transform;
			child.transform.localPosition = localPosition;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
		}

		public static void AlignAs(this GameObject obj, GameObject other)
		{
			obj.transform.position = other.transform.position;
			obj.transform.rotation = other.transform.rotation;
			obj.transform.localScale = other.transform.localScale;
		}

		public static void AlignAs(this GameObject obj, Transform other)
		{
			obj.transform.position = other.position;
			obj.transform.rotation = other.rotation;
			obj.transform.localScale = other.localScale;
		}

		public static string GetFullPath(this GameObject obj)
		{
			return obj.transform.GetFullPath();
		}

		public static string GetFullPath(this Transform obj)
		{
			var builder = new StringBuilder();

			builder.Append(obj.name);

			var p = obj.parent;

			while (p != null)
			{
				builder.Insert(0, p.name + "/");
				p = p.parent;
			}

			return builder.ToString();
		}

		#endregion

		#region Transform
		public static void SetX(this Transform tr, float x)
		{
			Vector3 t = tr.position;
			t.x = x;
			tr.position = t;
		}

		public static void SetY(this Transform tr, float y)
		{
			Vector3 t = tr.position;
			t.y = y;
			tr.position = t;
		}

		public static void SetZ(this Transform tr, float z)
		{
			Vector3 t = tr.position;
			t.z = z;
			tr.position = t;
		}

		public static void ShiftX(this Transform tr, float ox)
		{
			Vector3 t = tr.position;
			t.x += ox;
			tr.position = t;
		}

		public static void ShiftY(this Transform tr, float oy)
		{
			Vector3 t = tr.position;
			t.y += oy;
			tr.position = t;
		}

		public static void ShiftZ(this Transform tr, float oz)
		{
			Vector3 t = tr.position;
			t.z += oz;
			tr.position = t;
		}

		public static void SetLocalX(this Transform tr, float x)
		{
			Vector3 t = tr.localPosition;
			t.x = x;
			tr.localPosition = t;
		}

		public static void SetLocalY(this Transform tr, float y)
		{
			Vector3 t = tr.localPosition;
			t.y = y;
			tr.localPosition = t;
		}

		public static void SetLocalZ(this Transform tr, float z)
		{
			Vector3 t = tr.localPosition;
			t.z = z;
			tr.localPosition = t;
		}

		public static void ShiftLocalX(this Transform tr, float ox)
		{
			Vector3 t = tr.localPosition;
			t.x += ox;
			tr.localPosition = t;
		}

		public static void ShiftLocalY(this Transform tr, float oy)
		{
			Vector3 t = tr.localPosition;
			t.y += oy;
			tr.localPosition = t;
		}

		public static void ShiftLocalZ(this Transform tr, float oz)
		{
			Vector3 t = tr.localPosition;
			t.z += oz;
			tr.localPosition = t;
		}
		#endregion

		#region Input
		public static Vector2 GetFirstTouchOrMousePosition()
		{
			return Input.touchCount > 0 ?
				Input.touches[0].position :
				new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		}
		#endregion

		#region Color
		public static string ToJSON(this Color color)
		{
			ColorToJSONHelper colorToJSON = new ColorToJSONHelper(color);
			return JSON.Serialize(colorToJSON);
		}

		public static Color FromJSON(this Color color, string jsonString)
		{
			ColorToJSONHelper colorToJSON = new ColorToJSONHelper(jsonString);
			color.r = ((float)colorToJSON.R.Value) / 255f;
			color.g = ((float)colorToJSON.G.Value) / 255f;
			color.b = ((float)colorToJSON.B.Value) / 255f;
			color.a = ((float)colorToJSON.A.Value) / 255f;

			return color;
		}

		public class ColorToJSONHelper
		{
			public byte? R;
			public byte? G;
			public byte? B;
			public byte? A;

			public ColorToJSONHelper()
			{
			}

			public ColorToJSONHelper(Color color)
			{
				R = (byte)(color.r * 255f);
				G = (byte)(color.g * 255f);
				B = (byte)(color.b * 255f);
				A = (byte)(color.a * 255f);
			}

			public ColorToJSONHelper(string jsonString)
			{
				ColorToJSONHelper jsonHelper = JSON.Parse<ColorToJSONHelper>(jsonString);
				R = jsonHelper.R;
				G = jsonHelper.G;
				B = jsonHelper.B;
				A = jsonHelper.A;
			}
		}
		#endregion

		public static bool AddOnce<T>(this List<T> obj, T item)
		{
			if (obj.IndexOf(item) < 0)
			{
				obj.Add(item);
				return true;
			}
			else
			{
				return false;
			}
		}

		#region random
		public static bool RandomBool()
		{
			return UnityEngine.Random.Range(0, 2) == 1;
		}

		// random float [min, max] 
		public static float RandomFloat(float min, float max)
		{
			return UnityEngine.Random.Range(min, max);
		}

		// random int [min, max] 
		public static int RandomInt(int min, int max)
		{
			return UnityEngine.Random.Range(min, max + 1);
		}

		public static bool RandomBool(System.Random random)
		{
			return random.Next(0, 2) == 1;
		}

		// random float [min, max] 
		public static float RandomFloat(float min, float max, System.Random random)
		{
			return (float)(random.NextDouble() * (max - min) + min);
		}

		// random int [min, max] 
		public static int RandomInt(int min, int max, System.Random random)
		{
			return random.Next(min, max + 1);
		}

		public static int RandomIndex<T>(this List<T> obj)
		{
			var c = obj.Count;
			return (c > 0) ? UnityEngine.Random.Range(0, c) : -1;
		}

		public static T RandomItem<T>(this List<T> obj)
		{
			var c = obj.Count;
			if (c > 0)
				return obj[UnityEngine.Random.Range(0, c)];
			else
				return default(T);
		}

		public static T RandomItem<T>(this FastList<T> obj)
		{
			var c = obj.size;
			if (c > 0)
				return obj[UnityEngine.Random.Range(0, c)];
			else
				return default(T);
		}

		public static int RandomIndex<T>(this T[] obj)
		{
			var c = obj.Length;
			return (c > 0) ? UnityEngine.Random.Range(0, c) : -1;
		}

		public static T RandomItem<T>(this T[] obj)
		{
			var c = obj.Length;
			if (c > 0)
				return obj[UnityEngine.Random.Range(0, c)];
			else
				return default(T);
		}

		public static T RandomItem<T>(this List<T> obj, System.Random random)
		{
			var c = obj.Count;
			if (c > 0)
				return obj[random.Next(0, c)];
			else
				return default(T);
		}

		public static int RandomIndex<T>(this T[] obj, System.Random random)
		{
			var c = obj.Length;
			return (c > 0) ? random.Next(0, c) : -1;
		}

		public static T RandomItem<T>(this T[] obj, System.Random random)
		{
			var c = obj.Length;
			if (c > 0)
				return obj[random.Next(0, c)];
			else
				return default(T);
		}
		#endregion

		#region LogType
		public static bool IsPassedThrough(this LogType baseType, LogType? otherType)
		{
			if (otherType == null)
				return false;

			var other = otherType.Value;
			switch (baseType)
			{
				case LogType.Exception:
					return true;
				case LogType.Assert:
					return other != LogType.Exception;
				case LogType.Error:
					return other != LogType.Exception && other != LogType.Assert;
				case LogType.Warning:
					return other == LogType.Log || other == LogType.Warning;
				case LogType.Log:
					return other == LogType.Log;
			}

			return true;
		}
		#endregion

		#region String

		public static string ToRoman(this int number)
		{
			switch (number)
			{
				case 1:
					return "I";

				case 2:
					return "II";

				case 3:
					return "III";

				case 4:
					return "IV";

				case 5:
					return "V";

				default:
					return "X";

			}
		}

		#endregion

		public static DateTime ToDateTimeFromUnixTime(this long unixTimeMillis)
		{
			DateTime utcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return utcDateTime.AddMilliseconds(unixTimeMillis);
		}

		#region Matrices

		public static void Decompose(this Matrix4x4 m, out Vector3 p, out Quaternion q, out Vector3 s)
		{
			p = new Vector3(m.m03, m.m13, m.m23);

			var column0 = new Vector4(m.m00, m.m10, m.m20, m.m30);
			var column1 = new Vector4(m.m01, m.m11, m.m21, m.m31);
			var column2 = new Vector4(m.m02, m.m12, m.m22, m.m32);

			q = Quaternion.LookRotation(column2, column1);

			s = new Vector3(column0.magnitude, column1.magnitude, column2.magnitude);
		}

		public static Vector3 GetPosition(this Matrix4x4 m)
		{
			return new Vector3(m.m03, m.m13, m.m23);
		}

		public static Quaternion GetRotation(this Matrix4x4 m)
		{
			return Quaternion.LookRotation(
				new Vector4(m.m02, m.m12, m.m22, m.m32),
				new Vector4(m.m01, m.m11, m.m21, m.m31)
			);
		}

		public static Vector3 GetScale(this Matrix4x4 m)
		{
			return new Vector3(
				new Vector4(m.m00, m.m10, m.m20, m.m30).magnitude,
				new Vector4(m.m01, m.m11, m.m21, m.m31).magnitude,
				new Vector4(m.m02, m.m12, m.m22, m.m32).magnitude
			);
		}

		#endregion

		#region Vector2

		public static float Cross(this Vector2 lhs, Vector2 rhs)
		{
			return lhs.x * rhs.y - lhs.y * rhs.x;
		}

		#endregion
	}

	#region FlagsEditor
	public class EnumFlagsAttribute : PropertyAttribute
	{
		public EnumFlagsAttribute()
		{
		}
	}

#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : UnityEditor.PropertyDrawer
{
	public override void OnGUI(Rect _position, UnityEditor.SerializedProperty _property, GUIContent _label)
	{
		_property.intValue = UnityEditor.EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
	}
}
#endif
	#endregion
}