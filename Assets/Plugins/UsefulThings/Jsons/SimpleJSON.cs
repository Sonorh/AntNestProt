//#define USE_SharpZipLib

/* * * * *
 * A simple JSON Parser / builder
 * ------------------------------
 * 
 * It mainly has been written as a simple JSON parser. It can build a JSON string
 * from the node-tree, or generate a node tree from any valid JSON string.
 * 
 * If you want to use compression when saving to file / stream / B64 you have to include
 * SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ ) in your project and
 * define "USE_SharpZipLib" at the top of the file
 * 
 * Written by Bunny83 
 * 2012-06-09
 * 
 * Features / attributes:
 * - provides strongly typed node classes and lists / dictionaries
 * - provides easy access to class members / array items / data values
 * - the parser ignores data types. Each value is a string.
 * - only double quotes (") are used for quoting strings.
 * - values and names are not restricted to quoted strings. They simply add up and are trimmed.
 * - There are only 3 types: arrays(JSONArray), objects(JSONClass) and values(JSONData)
 * - provides "casting" properties to easily convert to / from those types:
 *   int / float / double / bool
 * - provides a common interface for each node so no explicit casting is required.
 * - the parser try to avoid errors, but if malformed JSON is parsed the result is undefined
 * 
 * 
 * 2012-12-17 Update:
 * - Added internal JSONLazyCreator class which simplifies the construction of a JSON tree
 *   Now you can simple reference any item that doesn't exist yet and it will return a JSONLazyCreator
 *   The class determines the required type by it's further use, creates the type and removes itself.
 * - Added binary serialization / deserialization.
 * - Added support for BZip2 zipped binary format. Requires the SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ )
 *   The usage of the SharpZipLib library can be disabled by removing or commenting out the USE_SharpZipLib define at the top
 * - The serializer uses different types when it comes to store the values. Since my data values
 *   are all of type string, the serializer will "try" which format fits best. The order is: int, float, double, bool, string.
 *   It's not the most efficient way but for a moderate amount of data it should work on all platforms.
 * 
 * * * * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PM.UsefulThings.SimpleJson
{
	public interface IJsonPostDeserializeCallback
	{
		void PostDeserialize();
	}

	public sealed class JsonIgnoreAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Field)]
	public sealed class JsonSerializeAttribute : Attribute
	{
	}

	public enum JSONBinaryTag
	{
		Array = 1,
		Class = 2,
		Value = 3,
		IntValue = 4,
		DoubleValue = 5,
		BoolValue = 6,
		FloatValue = 7,
	}

	public class JSONNode
	{
		#region common interface
		public virtual void Add(string aKey, JSONNode aItem) { }
		public virtual JSONNode this[int aIndex] { get { return null; } set { } }
		public virtual JSONNode this[string aKey] { get { return null; } set { } }
		public virtual string Value { get { return ""; } set { } }
		public virtual int Count { get { return 0; } }

		public virtual void Add(JSONNode aItem)
		{
			Add("", aItem);
		}

		public virtual JSONNode Remove(string aKey) { return null; }
		public virtual JSONNode Remove(int aIndex) { return null; }
		public virtual JSONNode Remove(JSONNode aNode) { return aNode; }
		public virtual bool HasKey(string key) { return false; }

		public virtual IEnumerable<JSONNode> Children { get { yield break; } }
		public IEnumerable<JSONNode> DeepChildren
		{
			get
			{
				foreach (var C in Children)
					foreach (var D in C.DeepChildren)
						yield return D;
			}
		}

		public virtual void ToString(ref StringBuilder sb)
		{
			sb.Append("JSONNode");
		}
		public virtual void ToString(ref StringBuilder sb, string aPrefix)
		{
			sb.Append("JSONNode");
		}

		#endregion common interface

		#region typecasting properties
		public virtual int AsInt
		{
			get
			{
				int v = 0;
				if (int.TryParse(Value, out v))
					return v;
				return 0;
			}
			set
			{
				Value = value.ToString();
			}
		}
		public virtual float AsFloat
		{
			get
			{
				float v = 0.0f;
				if (float.TryParse(Value, out v))
					return v;
				return 0.0f;
			}
			set
			{
				Value = value.ToString();
			}
		}
		public virtual double AsDouble
		{
			get
			{
				double v = 0.0;
				if (double.TryParse(Value, out v))
					return v;
				return 0.0;
			}
			set
			{
				Value = value.ToString();
			}
		}
		public virtual bool AsBool
		{
			get
			{
				bool v = false;
				if (bool.TryParse(Value, out v))
					return v;
				return !string.IsNullOrEmpty(Value);
			}
			set
			{
				Value = (value) ? "true" : "false";
			}
		}
		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}
		public virtual JSONClass AsObject
		{
			get
			{
				return this as JSONClass;
			}
		}


		#endregion typecasting properties

		#region operators
		public static implicit operator JSONNode(string s)
		{
			return new JSONData(s);
		}
		public static implicit operator string(JSONNode d)
		{
			return (d == null) ? null : d.Value;
		}
		public static bool operator ==(JSONNode a, object b)
		{
			if (b == null && a is JSONLazyCreator)
				return true;
			return System.Object.ReferenceEquals(a, b);
		}

		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}
		public override bool Equals(object obj)
		{
			return System.Object.ReferenceEquals(this, obj);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}


		#endregion operators

		internal static void Escape(ref StringBuilder sb, string aText)
		{
			foreach (char c in aText)
			{
				switch (c)
				{
					case '\\':
						sb.Append("\\\\");
						break;
					case '\"':
						sb.Append("\\\"");
						break;
					case '\n':
						sb.Append("\\n");
						break;
					case '\r':
						sb.Append("\\r");
						break;
					case '\t':
						sb.Append("\\t");
						break;
					case '\b':
						sb.Append("\\b");
						break;
					case '\f':
						sb.Append("\\f");
						break;
					default:
						sb.Append(c);
						break;
				}
			}
		}

		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode ctx = null;
			int i = 0;
			string Token = "";
			string TokenName = "";
			bool QuoteMode = false;
			while (i < aJSON.Length)
			{
				switch (aJSON[i])
				{
					case '{':
						if (QuoteMode)
						{
							Token += aJSON[i];
							break;
						}
						stack.Push(new JSONClass());
						if (ctx != null)
						{
							TokenName = TokenName.Trim();
							if (ctx is JSONArray)
								ctx.Add(stack.Peek());
							else if (TokenName != "")
								ctx.Add(TokenName, stack.Peek());
						}
						TokenName = "";
						Token = "";
						ctx = stack.Peek();
						break;

					case '[':
						if (QuoteMode)
						{
							Token += aJSON[i];
							break;
						}

						stack.Push(new JSONArray());
						if (ctx != null)
						{
							TokenName = TokenName.Trim();
							if (ctx is JSONArray)
								ctx.Add(stack.Peek());
							else if (TokenName != "")
								ctx.Add(TokenName, stack.Peek());
						}
						TokenName = "";
						Token = "";
						ctx = stack.Peek();
						break;

					case '}':
					case ']':
						if (QuoteMode)
						{
							Token += aJSON[i];
							break;
						}
						if (stack.Count == 0)
							throw new Exception("JSON Parse: Too many closing brackets");

						stack.Pop();
						if (Token != "")
						{
							TokenName = TokenName.Trim();
							if (ctx is JSONArray)
								ctx.Add(Token);
							else if (TokenName != "")
								ctx.Add(TokenName, Token);
						}
						TokenName = "";
						Token = "";
						if (stack.Count > 0)
							ctx = stack.Peek();
						break;

					case ':':
						if (QuoteMode)
						{
							Token += aJSON[i];
							break;
						}
						TokenName = Token;
						Token = "";
						break;

					case '"':
						QuoteMode ^= true;
						break;

					case ',':
						if (QuoteMode)
						{
							Token += aJSON[i];
							break;
						}
						if (Token != "")
						{
							if (ctx is JSONArray)
								ctx.Add(Token);
							else if (TokenName != "")
								ctx.Add(TokenName, Token);
						}
						TokenName = "";
						Token = "";
						break;

					case '\r':
					case '\n':
						break;

					case ' ':
					case '\t':
						if (QuoteMode)
							Token += aJSON[i];
						break;

					case '\\':
						++i;
						if (QuoteMode)
						{
							char C = aJSON[i];
							switch (C)
							{
								case 't':
									Token += '\t';
									break;
								case 'r':
									Token += '\r';
									break;
								case 'n':
									Token += '\n';
									break;
								case 'b':
									Token += '\b';
									break;
								case 'f':
									Token += '\f';
									break;
								case 'u':
									{
										string s = aJSON.Substring(i + 1, 4);
										Token += (char)int.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
										i += 4;
										break;
									}
								default:
									Token += C;
									break;
							}
						}
						break;

					default:
						Token += aJSON[i];
						break;
				}
				++i;
			}
			if (QuoteMode)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return ctx;
		}

		public virtual void Serialize(System.IO.BinaryWriter aWriter) { }

		public void SaveToStream(System.IO.Stream aData)
		{
			var W = new System.IO.BinaryWriter(aData);
			Serialize(W);
		}

#if USE_SharpZipLib
		public void SaveToCompressedStream(System.IO.Stream aData)
		{
			using (var gzipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(aData))
			{
				gzipOut.IsStreamOwner = false;
				SaveToStream(gzipOut);
				gzipOut.Close();
			}
		}
		
		public void SaveToCompressedFile(string aFileName)
		{
			System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
			using(var F = System.IO.File.OpenWrite(aFileName))
			{
				SaveToCompressedStream(F);
			}
		}
		public string SaveToCompressedBase64()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				SaveToCompressedStream(stream);
				stream.Position = 0;
				return System.Convert.ToBase64String(stream.ToArray());
			}
		}
		
#else
		public void SaveToCompressedStream(System.IO.Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
		public string SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
#endif

		public void SaveToFile(string aFileName)
		{
			System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
			using (var F = System.IO.File.OpenWrite(aFileName))
			{
				SaveToStream(F);
			}
		}
		public string SaveToBase64()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				SaveToStream(stream);
				stream.Position = 0;
				return System.Convert.ToBase64String(stream.ToArray());
			}
		}
		public static JSONNode Deserialize(System.IO.BinaryReader aReader)
		{
			JSONBinaryTag type = (JSONBinaryTag)aReader.ReadByte();
			switch (type)
			{
				case JSONBinaryTag.Array:
					{
						int count = aReader.ReadInt32();
						JSONArray tmp = new JSONArray();
						for (int i = 0; i < count; i++)
							tmp.Add(Deserialize(aReader));
						return tmp;
					}
				case JSONBinaryTag.Class:
					{
						int count = aReader.ReadInt32();
						JSONClass tmp = new JSONClass();
						for (int i = 0; i < count; i++)
						{
							string key = aReader.ReadString();
							var val = Deserialize(aReader);
							tmp.Add(key, val);
						}
						return tmp;
					}
				case JSONBinaryTag.Value:
					{
						return new JSONData(aReader.ReadString());
					}
				case JSONBinaryTag.IntValue:
					{
						return new JSONData(aReader.ReadInt32());
					}
				case JSONBinaryTag.DoubleValue:
					{
						return new JSONData(aReader.ReadDouble());
					}
				case JSONBinaryTag.BoolValue:
					{
						return new JSONData(aReader.ReadBoolean());
					}
				case JSONBinaryTag.FloatValue:
					{
						return new JSONData(aReader.ReadSingle());
					}

				default:
					{
						throw new Exception("Error deserializing JSON. Unknown tag: " + type);
					}
			}
		}

#if USE_SharpZipLib
		public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
		{
			var zin = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(aData);
			return LoadFromStream(zin);
		}
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			using(var F = System.IO.File.OpenRead(aFileName))
			{
				return LoadFromCompressedStream(F);
			}
		}
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			var tmp = System.Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromCompressedStream(stream);
		}
#else
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
		public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
#endif

		public static JSONNode LoadFromStream(System.IO.Stream aData)
		{
			using (var R = new System.IO.BinaryReader(aData))
			{
				return Deserialize(R);
			}
		}
		public static JSONNode LoadFromFile(string aFileName)
		{
			using (var F = System.IO.File.OpenRead(aFileName))
			{
				return LoadFromStream(F);
			}
		}
		public static JSONNode LoadFromBase64(string aBase64)
		{
			var tmp = System.Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromStream(stream);
		}
	} // End of JSONNode

	public class JSONArray : JSONNode, IEnumerable
	{
		private List<JSONNode> m_List = new List<JSONNode>();
		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= m_List.Count)
					return new JSONLazyCreator(this);
				return m_List[aIndex];
			}
			set
			{
				if (aIndex < 0 || aIndex >= m_List.Count)
					m_List.Add(value);
				else
					m_List[aIndex] = value;
			}
		}
		public override JSONNode this[string aKey]
		{
			get { return new JSONLazyCreator(this); }
			set { m_List.Add(value); }
		}
		public override int Count
		{
			get { return m_List.Count; }
		}
		public override void Add(string aKey, JSONNode aItem)
		{
			m_List.Add(aItem);
		}
		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= m_List.Count)
				return null;
			JSONNode tmp = m_List[aIndex];
			m_List.RemoveAt(aIndex);
			return tmp;
		}
		public override JSONNode Remove(string aKey)
		{
			for (int i = 0; i < m_List.Count; i++)
			{
				if (m_List[i].Value == aKey)
				{
					JSONNode tmp = m_List[i];
					m_List.RemoveAt(i);
					return tmp;
				}
			}
			return null;
		}
		public override JSONNode Remove(JSONNode aNode)
		{
			m_List.Remove(aNode);
			return aNode;
		}
		public override IEnumerable<JSONNode> Children
		{
			get
			{
				foreach (JSONNode N in m_List)
					yield return N;
			}
		}
		public IEnumerator GetEnumerator()
		{
			foreach (JSONNode N in m_List)
				yield return N;
		}
		public override void ToString(ref StringBuilder sb)
		{
			sb.Append("[ ");
			bool secondIteration = false;
			for (int i = 0, iMax = m_List.Count; i < iMax; i++)
			{
				if (secondIteration)
				{
					sb.Append(", ");
				}
				secondIteration = true;
				m_List[i].ToString(ref sb);
			}
			sb.Append(" ]");
		}
		public override void ToString(ref StringBuilder sb, string aPrefix)
		{
			sb.Append("[ ");
			bool secondIteration = false;
			for (int i = 0, iMax = m_List.Count; i < iMax; i++)
			{
				if (secondIteration)
				{
					sb.Append(", ");
				}
				secondIteration = true;
				sb.Append("\n");
				sb.Append(aPrefix);
				sb.Append("   ");
				m_List[i].ToString(ref sb, aPrefix + "   ");
			}
			sb.Append("\n");
			sb.Append(aPrefix);
			sb.Append("]");
		}
		public override void Serialize(System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)JSONBinaryTag.Array);
			aWriter.Write(m_List.Count);
			for (int i = 0; i < m_List.Count; i++)
			{
				m_List[i].Serialize(aWriter);
			}
		}
	} // End of JSONArray

	public class JSONClass : JSONNode, IEnumerable
	{
		private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();

		public Dictionary<string, JSONNode>.KeyCollection Keys
		{
			get { return m_Dict.Keys; }
		}

		public override JSONNode this[string aKey]
		{
			get
			{
				if (m_Dict.ContainsKey(aKey))
					return m_Dict[aKey];
				else
					return new JSONLazyCreator(this, aKey);
			}
			set
			{
				if (m_Dict.ContainsKey(aKey))
					m_Dict[aKey] = value;
				else
					m_Dict.Add(aKey, value);
			}
		}
		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= m_Dict.Count)
					return null;
				return m_Dict.ElementAt(aIndex).Value;
			}
			set
			{
				if (aIndex < 0 || aIndex >= m_Dict.Count)
					return;
				string key = m_Dict.ElementAt(aIndex).Key;
				m_Dict[key] = value;
			}
		}
		public override int Count
		{
			get { return m_Dict.Count; }
		}


		public override void Add(string aKey, JSONNode aItem)
		{
			if (!string.IsNullOrEmpty(aKey))
			{
				if (m_Dict.ContainsKey(aKey))
					m_Dict[aKey] = aItem;
				else
					m_Dict.Add(aKey, aItem);
			}
			else
				m_Dict.Add(Guid.NewGuid().ToString(), aItem);
		}

		public override JSONNode Remove(string aKey)
		{
			if (!m_Dict.ContainsKey(aKey))
				return null;
			JSONNode tmp = m_Dict[aKey];
			m_Dict.Remove(aKey);
			return tmp;
		}
		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= m_Dict.Count)
				return null;
			var item = m_Dict.ElementAt(aIndex);
			m_Dict.Remove(item.Key);
			return item.Value;
		}
		public override JSONNode Remove(JSONNode aNode)
		{
			try
			{
				var item = m_Dict.Where(k => k.Value == aNode).First();
				m_Dict.Remove(item.Key);
				return aNode;
			}
			catch
			{
				return null;
			}
		}

		public override bool HasKey(string key)
		{
			return m_Dict.ContainsKey(key);
		}

		public override IEnumerable<JSONNode> Children
		{
			get
			{
				foreach (KeyValuePair<string, JSONNode> N in m_Dict)
					yield return N.Value;
			}
		}

		public IEnumerator GetEnumerator()
		{
			foreach (KeyValuePair<string, JSONNode> N in m_Dict)
				yield return N;
		}
		public override void ToString(ref StringBuilder sb)
		{
			sb.Append("{");
			bool secondIteration = false;
			foreach (KeyValuePair<string, JSONNode> N in m_Dict)
			{
				if (secondIteration)
				{
					sb.Append(", ");
				}
				secondIteration = true;
				sb.Append("\"");
				Escape(ref sb, N.Key);
				sb.Append("\":");
				N.Value.ToString(ref sb);
			}
			sb.Append("}");
		}
		public override void ToString(ref StringBuilder sb, string aPrefix)
		{
			sb.Append("{ ");
			bool secondIteration = false;
			foreach (KeyValuePair<string, JSONNode> N in m_Dict)
			{
				if (secondIteration)
				{
					sb.Append(", ");
				}
				secondIteration = true;
				sb.Append("\n");
				sb.Append(aPrefix);
				sb.Append("   ");
				sb.Append("\"");
				Escape(ref sb, N.Key);
				sb.Append("\" : ");
				N.Value.ToString(ref sb, aPrefix + "   ");
			}
			sb.Append("\n");
			sb.Append(aPrefix);
			sb.Append("}");
		}
		public override void Serialize(System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)JSONBinaryTag.Class);
			aWriter.Write(m_Dict.Count);
			foreach (string K in m_Dict.Keys)
			{
				aWriter.Write(K);
				m_Dict[K].Serialize(aWriter);
			}
		}
	} // End of JSONClass

	public class JSONData : JSONNode
	{
		private string m_Data;
		public override string Value
		{
			get { return m_Data; }
			set { m_Data = value; }
		}
		public JSONData(string aData)
		{
			m_Data = aData;
		}
		public JSONData(float aData)
		{
			AsFloat = aData;
		}
		public JSONData(double aData)
		{
			AsDouble = aData;
		}
		public JSONData(bool aData)
		{
			AsBool = aData;
		}
		public JSONData(int aData)
		{
			AsInt = aData;
		}

		public override void ToString(ref StringBuilder sb)
		{
			sb.Append("\"");
			Escape(ref sb, m_Data);
			sb.Append("\"");
		}
		public override void ToString(ref StringBuilder sb, string aPrefix)
		{
			sb.Append("\"");
			Escape(ref sb, m_Data);
			sb.Append("\"");
		}
		public override void Serialize(System.IO.BinaryWriter aWriter)
		{
			var tmp = new JSONData("");

			tmp.AsInt = AsInt;
			if (tmp.m_Data == this.m_Data)
			{
				aWriter.Write((byte)JSONBinaryTag.IntValue);
				aWriter.Write(AsInt);
				return;
			}
			tmp.AsFloat = AsFloat;
			if (tmp.m_Data == this.m_Data)
			{
				aWriter.Write((byte)JSONBinaryTag.FloatValue);
				aWriter.Write(AsFloat);
				return;
			}
			tmp.AsDouble = AsDouble;
			if (tmp.m_Data == this.m_Data)
			{
				aWriter.Write((byte)JSONBinaryTag.DoubleValue);
				aWriter.Write(AsDouble);
				return;
			}

			tmp.AsBool = AsBool;
			if (tmp.m_Data == this.m_Data)
			{
				aWriter.Write((byte)JSONBinaryTag.BoolValue);
				aWriter.Write(AsBool);
				return;
			}
			aWriter.Write((byte)JSONBinaryTag.Value);
			aWriter.Write(m_Data);
		}
	} // End of JSONData

	internal class JSONLazyCreator : JSONNode
	{
		private JSONNode m_Node = null;
		private string m_Key = null;

		public JSONLazyCreator(JSONNode aNode)
		{
			m_Node = aNode;
			m_Key = null;
		}
		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			m_Node = aNode;
			m_Key = aKey;
		}

		private void Set(JSONNode aVal)
		{
			if (m_Key == null)
			{
				m_Node.Add(aVal);
			}
			else
			{
				m_Node.Add(m_Key, aVal);
			}
			m_Node = null; // Be GC friendly.
		}

		public override JSONNode this[int aIndex]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				var tmp = new JSONArray();
				tmp.Add(value);
				Set(tmp);
			}
		}

		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				var tmp = new JSONClass();
				tmp.Add(aKey, value);
				Set(tmp);
			}
		}
		public override void Add(JSONNode aItem)
		{
			var tmp = new JSONArray();
			tmp.Add(aItem);
			Set(tmp);
		}
		public override void Add(string aKey, JSONNode aItem)
		{
			var tmp = new JSONClass();
			tmp.Add(aKey, aItem);
			Set(tmp);
		}
		public static bool operator ==(JSONLazyCreator a, object b)
		{
			if (b == null)
				return true;
			return System.Object.ReferenceEquals(a, b);
		}

		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}
		public override bool Equals(object obj)
		{
			if (obj == null)
				return true;
			return System.Object.ReferenceEquals(this, obj);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override void ToString(ref StringBuilder sb)
		{
			sb.Append("");
		}
		public override void ToString(ref StringBuilder sb, string aPrefix)
		{
			sb.Append("");
		}

		public override int AsInt
		{
			get
			{
				JSONData tmp = new JSONData(0);
				Set(tmp);
				return 0;
			}
			set
			{
				JSONData tmp = new JSONData(value);
				Set(tmp);
			}
		}
		public override float AsFloat
		{
			get
			{
				JSONData tmp = new JSONData(0.0f);
				Set(tmp);
				return 0.0f;
			}
			set
			{
				JSONData tmp = new JSONData(value);
				Set(tmp);
			}
		}
		public override double AsDouble
		{
			get
			{
				JSONData tmp = new JSONData(0.0);
				Set(tmp);
				return 0.0;
			}
			set
			{
				JSONData tmp = new JSONData(value);
				Set(tmp);
			}
		}
		public override bool AsBool
		{
			get
			{
				JSONData tmp = new JSONData(false);
				Set(tmp);
				return false;
			}
			set
			{
				JSONData tmp = new JSONData(value);
				Set(tmp);
			}
		}
		public override JSONArray AsArray
		{
			get
			{
				JSONArray tmp = new JSONArray();
				Set(tmp);
				return tmp;
			}
		}
		public override JSONClass AsObject
		{
			get
			{
				JSONClass tmp = new JSONClass();
				Set(tmp);
				return tmp;
			}
		}
	} // End of JSONLazyCreator

	internal class JsonFormatter
	{
		public class StringWalker
		{
			private readonly string _s;

			public int Index { get; private set; }
			public bool IsEscaped { get; private set; }
			public char CurrentChar { get; private set; }

			public StringWalker(string s)
			{
				_s = s;
				this.Index = -1;
			}

			public bool MoveNext()
			{
				if (this.Index == _s.Length - 1)
					return false;

				if (IsEscaped == false)
					IsEscaped = CurrentChar == '\\';
				else
					IsEscaped = false;
				this.Index++;
				CurrentChar = _s[Index];
				return true;
			}
		};

		public class IndentWriter
		{
			private readonly StringBuilder _result = new StringBuilder();
			private int _indentLevel;

			public void Indent()
			{
				_indentLevel++;
			}

			public void UnIndent()
			{
				if (_indentLevel > 0)
					_indentLevel--;
			}

			public void WriteLine(string line)
			{
				_result.AppendLine(CreateIndent() + line);
			}

			private string CreateIndent()
			{
				var indent = new StringBuilder();
				for (int i = 0; i < _indentLevel; i++)
					indent.Append("    ");
				return indent.ToString();
			}

			public override string ToString()
			{
				return _result.ToString();
			}
		};

		private readonly StringWalker _walker;
		private readonly IndentWriter _writer = new IndentWriter();
		private readonly StringBuilder _currentLine = new StringBuilder();
		private bool _quoted;

		public JsonFormatter(string json)
		{
			_walker = new StringWalker(json);
			ResetLine();
		}

		public void ResetLine()
		{
			_currentLine.Length = 0;
		}

		public string Format()
		{
			while (MoveNextChar())
			{
				if (this._quoted == false && this.IsOpenBracket())
				{
					this.WriteCurrentLine();
					this.AddCharToLine();
					this.WriteCurrentLine();
					_writer.Indent();
				}
				else if (this._quoted == false && this.IsCloseBracket())
				{
					this.WriteCurrentLine();
					_writer.UnIndent();
					this.AddCharToLine();
				}
				else if (this._quoted == false && this.IsColon())
				{
					this.AddCharToLine();
					this.WriteCurrentLine();
				}
				else
				{
					AddCharToLine();
				}
			}
			this.WriteCurrentLine();
			return _writer.ToString();
		}

		private bool MoveNextChar()
		{
			bool success = _walker.MoveNext();
			if (this.IsApostrophe())
			{
				this._quoted = !_quoted;
			}
			return success;
		}

		public bool IsApostrophe()
		{
			return this._walker.CurrentChar == '"' && this._walker.IsEscaped == false;
		}

		public bool IsOpenBracket()
		{
			return this._walker.CurrentChar == '{'
				|| this._walker.CurrentChar == '[';
		}

		public bool IsCloseBracket()
		{
			return this._walker.CurrentChar == '}'
				|| this._walker.CurrentChar == ']';
		}

		public bool IsColon()
		{
			return this._walker.CurrentChar == ',';
		}

		private void AddCharToLine()
		{
			this._currentLine.Append(_walker.CurrentChar);
		}

		private void WriteCurrentLine()
		{
			string line = this._currentLine.ToString().Trim();
			if (line.Length > 0)
			{
				_writer.WriteLine(line);
			}
			this.ResetLine();
		}
	}

	public static class JSON
	{
		private const string CLASS_NAME_FIELD = "__class";
		private const string DATETIME_FORMAT_STRING = "yyyy-MM-dd HH:mm:ss.ffff";

		private static StringBuilder m_StringBuilder = new StringBuilder();
		private static readonly Dictionary<char, List<KeyValuePair<string, Type>>> m_cachedTypesDictionary;

		static JSON()
		{
			m_cachedTypesDictionary = new Dictionary<char, List<KeyValuePair<string, Type>>>();

			var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(arg => arg.GetTypes());

			foreach (var type in types)
			{
				var typeName = type.Name;
				var firstLetter = typeName[0];

				if (!m_cachedTypesDictionary.ContainsKey(firstLetter))
				{
					m_cachedTypesDictionary.Add(firstLetter, new List<KeyValuePair<string, Type>>());
				}

				m_cachedTypesDictionary[firstLetter].Add(new KeyValuePair<string, Type>(typeName, type));
			}
		}

		public static JSONNode Parse(string aJSON)
		{
			return JSONNode.Parse(aJSON);
		}

		public static JSONNode TryParse(string aJSON, bool logError)
		{
			try
			{
				return JSONNode.Parse(aJSON);
			}
			catch
			{
				return null;
			}
		}

		public static T Parse<T>(string aJSON)
		{
			if (aJSON == null)
				return default(T);

			var rootNode = Parse(aJSON);

			var result = GetValueFromNode(rootNode, typeof(T));

			return (result is T) ? (T)result : default(T);
		}

		public static void Parse<T>(string aJSON, T obj) where T : class
		{
			if (obj == null)
			{
				return;
			}

			var rootNode = Parse(aJSON);
			GetObjectFromNode(rootNode, obj);
		}

		public static bool TryParseFromNode(JSONNode json, Type toType, out object result, bool logError = false)
		{
			result = Activator.CreateInstance(toType);

			if (json == null)
				return false;

			try
			{
				result = GetValueFromNode(json, toType);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool TryParseFromNode<T>(JSONNode json, out T result, bool logError = false)
		{
			object uncastedResult = null;
			bool success = TryParseFromNode(json, typeof(T), out uncastedResult, logError);

			result = (T)uncastedResult;
			return success;
		}

		public static bool TryParse(string json, Type toType, out object result, bool logError = false)
		{
			result = Activator.CreateInstance(toType);

			if (json == null)
				return false;

			try
			{
				var rootNode = Parse(json);

				result = GetValueFromNode(rootNode, toType);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool TryParse(string json, Type toType, out object result, out JSONNode rootNode, bool logError = false)
		{
			result = Activator.CreateInstance(toType);
			rootNode = null;

			if (json == null)
				return false;

			try
			{
				rootNode = Parse(json);

				result = GetValueFromNode(rootNode, toType);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool TryParse<T>(string aJSON, out T result, bool logError = false)
		{
			object uncastedResult = null;
			bool success = TryParse(aJSON, typeof(T), out uncastedResult, logError);

			result = (T)uncastedResult;
			return success;
		}

		/// <summary>
		/// Parses the JSON provided while skipping errors if possible (fails completely on wrong JSON).
		/// </summary>
		public static T ParseAndSkipErrors<T>(string aJSON)
		{
			try
			{
				var rootNode = Parse(aJSON);

				return (T)GetValueFromNode(rootNode, typeof(T), true);
			}
			catch
			{
				return default(T);
			}
		}

		public static string Serialize(object obj)
		{
			var rootNode = GetNodeFromValue(obj);
			if (rootNode == null)
			{
				return "";
			}
			rootNode.ToString(ref m_StringBuilder);
			var s = m_StringBuilder.ToString();
			m_StringBuilder.Length = 0;
			return s;
		}

		public static string Format(string json)
		{
			if (!string.IsNullOrEmpty(json))
			{
				var formatter = new JsonFormatter(json);
				return formatter.Format();
			}
			return json;
		}

		private static bool TryGetValueFromNode(JSONNode fromNode, Type withType, bool skipErrors, ref object result)
		{
			try
			{
				result = GetValueFromNode(fromNode, withType, skipErrors);
				return true;
			}
			catch (Exception e)
			{
				if (skipErrors)
					return false;
				else
				{
					Debug.LogError(e.Message + "\n" + e.StackTrace);
					return false;
				}
			}
		}

		private static void GetObjectFromNode(JSONNode fromNode, object result)
		{
			if (fromNode == null)
			{
				return;
			}

			var nullValue = "null".Equals(fromNode.Value, StringComparison.InvariantCultureIgnoreCase);
			var withType = result.GetType();

			// replace nullable type with its contents
			if (withType.IsGenericType && withType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				if (nullValue)
				{
					return;
				}

				withType = withType.GetGenericArguments()[0];
			}

			// JSONClass handling
			if (nullValue)
			{
				return;
			}

			var classNode = fromNode as JSONClass;
			if (classNode == null)
			{
				throw new Exception("Type mismatch - expected a class node but found " + fromNode.ToString());
			}

			// common case (class / struct)
			if (classNode.HasKey(CLASS_NAME_FIELD))
			{
				var incomingTypeName = classNode[CLASS_NAME_FIELD];
				var types = GetAllTypesByName(incomingTypeName);
				if (types.Count == 0)
				{
					throw new Exception("Cannot find requested type to deserialize - " + incomingTypeName);
				}

				if (types.Count > 1)
				{
					var str = "[ ";
					foreach (var t in types)
					{
						str += t.FullName + ' ';
					}
					str += "]";
					throw new Exception("Class name ambiguity: found " + types.Count + " types with names " + str);
				}

				var incomingType = types[0];
				if (!(withType.IsAssignableFrom(incomingType)))
					throw new Exception("Type mismatch - field with type " + withType.Name + " cannot be assigned from incoming type " +
										incomingType + " (" + (incomingType == null ? "" : incomingType.Name) + ")");

				withType = incomingType;
			}

			foreach (var field in withType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (!classNode.HasKey(field.Name))
				{
					continue;
				}

				object fieldValue = null;
				if (!TryGetValueFromNode(classNode[field.Name], field.FieldType, false, ref fieldValue))
				{
					continue;
				}

				field.SetValue(result, fieldValue);
			}

			foreach (var property in withType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				// indexed property or there is no incoming value for it or it has no setter
				if (property.GetIndexParameters().Length > 0 || !classNode.HasKey(property.Name) || !property.CanRead || !property.CanWrite)
				{
					continue;
				}

				object propertyValue = null;
				if (!TryGetValueFromNode(classNode[property.Name], property.PropertyType, false, ref propertyValue))
				{
					continue;
				}

				property.SetValue(result, propertyValue, null);
			}
		}

		private static object GetValueFromNode(JSONNode fromNode, Type withType, bool skipErrors = false)
		{
			if (fromNode == null)
				return null;

			var nullValue = "null".Equals(fromNode.Value, StringComparison.InvariantCultureIgnoreCase);

			// replace nullable type with its contents
			if (withType.IsGenericType && withType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				if (nullValue)
					return null;

				withType = withType.GetGenericArguments()[0];
			}

			bool typeIsBool = withType == typeof(bool);
			bool typeIsByte = withType == typeof(byte);
			bool typeIsInt = withType == typeof(int);
			bool typeIsLong = withType == typeof(long);
			bool typeIsULong = withType == typeof(ulong);
			bool typeIsFloat = withType == typeof(float);
			bool typeIsDouble = withType == typeof(double);
			bool typeIsString = withType == typeof(string);
			bool typeIsDateTime = withType == typeof(DateTime);
			bool typeIsArray = withType.IsArray;
			bool typeIsList = withType.IsGenericType && withType.GetGenericTypeDefinition() == typeof(List<>);
			bool typeIsSet = withType.IsGenericType && withType.GetGenericTypeDefinition() == typeof(HashSet<>);
			bool typeIsEnum = withType.IsEnum;

			if (!(fromNode is JSONData) &&
				(typeIsBool || typeIsInt || typeIsByte || typeIsFloat || typeIsDouble || typeIsString || typeIsDateTime))
				throw new Exception("Type mismatch - expected '" + withType.Name + "' but found " + fromNode.ToString());

			if (!(fromNode is JSONArray) && (typeIsArray || typeIsList || typeIsSet))
				throw new Exception("Type mismatch - expected an array but found " + fromNode.ToString());

			if (typeIsBool)
				return fromNode.AsBool;
			if (typeIsByte)
				return (byte)fromNode.AsInt;
			if (typeIsInt)
				return fromNode.AsInt;
			if (typeIsLong)
				return long.Parse(fromNode.Value);
			if (typeIsULong)
				return ulong.Parse(fromNode.Value);
			if (typeIsFloat)
				return fromNode.AsFloat;
			if (typeIsDouble)
				return fromNode.AsDouble;
			if (typeIsString)
				return fromNode.Value;
			if (typeIsDateTime)
				return ParseDateTime(fromNode.Value);

			if (typeIsEnum && fromNode is JSONData)
			{
				if (!Enum.IsDefined(withType, fromNode.Value))
					throw new Exception("Wrong enum value: '" + fromNode.Value + "' is not defined in " + withType.Name + " enum");

				// if enum value is encoded 'the old way', default branch (class) will try to handle it
				return Enum.Parse(withType, fromNode.Value);
			}

			if (typeIsArray)
			{
				var jsonArray = fromNode.AsArray;

				int counter = 0;
				var resultArray = Array.CreateInstance(withType.GetElementType(), jsonArray.Count);
				foreach (var node in jsonArray.Children)
				{
					object nodeValue = null;
					if (!TryGetValueFromNode(node, withType.GetElementType(), skipErrors, ref nodeValue))
						continue;

					resultArray.SetValue(nodeValue, counter++);
				}
				return resultArray;
			}

			// list
			if (typeIsList || typeIsSet)
			{
				var jsonArray = fromNode.AsArray;

				var valueType = withType.GetGenericArguments()[0];
				var addMethod = withType.GetMethod("Add");
				var resultList = Activator.CreateInstance(withType);
				foreach (var node in jsonArray.Children)
				{
					object nodeValue = null;
					if (!TryGetValueFromNode(node, valueType, skipErrors, ref nodeValue))
						continue;
					addMethod.Invoke(resultList, new object[] { nodeValue });
				}
				return resultList;
			}

			// JSONClass handling
			if (nullValue)
				return null;

			var classNode = fromNode as JSONClass;
			if (classNode == null)
				throw new Exception("Type mismatch - expected a class node but found " + fromNode.ToString());

			// dictionary
			if (withType.IsGenericType && withType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
			{
				var keyType = withType.GetGenericArguments()[0];
				var valueType = withType.GetGenericArguments()[1];

				var addMethod = withType.GetMethod("Add");
				var resultDictionary = Activator.CreateInstance(withType);
				foreach (string keyString in classNode.Keys)
				{
					var fakeKeyNode = new JSONData(keyString);
					var valueNode = classNode[keyString];

					object key = null;
					if (!TryGetValueFromNode(fakeKeyNode, keyType, skipErrors, ref key))
						continue;
					object value = null;
					if (!TryGetValueFromNode(valueNode, valueType, skipErrors, ref value))
						continue;

					addMethod.Invoke(resultDictionary, new object[] { key, value });
				}

				return resultDictionary;
			}

			// common case (class / struct)
			if (classNode.HasKey(CLASS_NAME_FIELD))
			{
				var incomingTypeName = classNode[CLASS_NAME_FIELD];
				var types = GetAllTypesByName(incomingTypeName);
				if (types.Count == 0)
					throw new Exception("Cannot find requested type to deserialize - " + incomingTypeName);

				if (types.Count == 1)
				{
					var incomingType = types[0];
					if (withType.IsAssignableFrom(incomingType))
					{
						withType = incomingType;
					}
					else
					{
						throw new Exception("Type mismatch - field with type " + withType.Name + " cannot be assigned from incoming type " +
							incomingType + " (" + (incomingType == null ? "" : incomingType.Name) + ")");
					}
				}
				else
				{
					var findedAssignable = false;
					foreach (var t in types)
					{
						if (withType.IsAssignableFrom(t))
						{
							withType = t;
							findedAssignable = true;
							break;
						}
					}
					if (!findedAssignable)
					{
						throw new Exception("Not finded candidat type: " + withType.Name);
					}
				}
			}

			var result = Activator.CreateInstance(withType);
			foreach (var field in withType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (!classNode.HasKey(field.Name))
					continue;

				object fieldValue = null;
				if (!TryGetValueFromNode(classNode[field.Name], field.FieldType, skipErrors, ref fieldValue))
					continue;

				field.SetValue(result, fieldValue);
			}

			foreach (var property in withType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				// indexed property or there is no incoming value for it or it has no setter
				if (property.GetIndexParameters().Length > 0 || !classNode.HasKey(property.Name) || !property.CanRead || !property.CanWrite)
					continue;

				object propertyValue = null;
				if (!TryGetValueFromNode(classNode[property.Name], property.PropertyType, skipErrors, ref propertyValue))
					continue;

				property.SetValue(result, propertyValue, null);
			}

			if (result is IJsonPostDeserializeCallback)
			{
				(result as IJsonPostDeserializeCallback).PostDeserialize();
			}

			return result;
		}

		private static JSONNode GetNodeFromValue(object obj)
		{
			if (obj == null)
				return new JSONData("null");

			Type objType = obj.GetType();
			if (objType.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length > 0)
			{
				return new JSONData("null");
			}

			if (typeof(Delegate).IsAssignableFrom(objType))
			{
				Debug.Log("JSON -> Serialize: Delegate type found: " + obj.ToString() + "! Ignoring..");
				return new JSONData("null");
			}

			// replace nullable type with its contents
			if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				var valueProperty = objType.GetProperty("Value");
				var valueGetMethod = valueProperty == null ? null : valueProperty.GetGetMethod();
				obj = valueGetMethod == null ? null : valueGetMethod.Invoke(obj, null);

				if (obj == null)
					return new JSONData("null");

				objType = obj.GetType();
			}

			if (obj is bool ||
				obj is byte || obj is int || obj is long || obj is ulong ||
				obj is float || obj is double ||
				obj is string ||
				objType.IsEnum)
			{
				return new JSONData(obj.ToString());
			}

			if (obj is DateTime)
			{
				return new JSONData(((DateTime)obj).ToString(DATETIME_FORMAT_STRING));
			}

			if (obj is Array)
			{
				var objArray = (Array)obj;

				var resultArray = new JSONArray();

				for (int i = 0; i < objArray.Length; i++)
				{
					resultArray.Add(GetNodeFromValue(objArray.GetValue(i)));
				}

				return resultArray;
			}

			// list
			if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(List<>))
			{
				var objList = (IList)obj;

				var resultList = new JSONArray();

				foreach (var element in objList)
				{
					resultList.Add(GetNodeFromValue(element));
				}

				return resultList;
			}

			if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(HashSet<>))
			{
				var objList = (IEnumerable)obj;

				var resultList = new JSONArray();

				foreach (var element in objList)
				{
					resultList.Add(GetNodeFromValue(element));
				}

				return resultList;
			}

			// dictionary
			if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
			{
				var resultDictionary = new JSONClass();

				IDictionary objDictionary = (IDictionary)obj;
				foreach (var key in objDictionary.Keys)
				{
					string keyString = key.ToString();
					JSONNode valueNode = GetNodeFromValue(objDictionary[key]);

					resultDictionary.Add(keyString, valueNode);
				}

				return resultDictionary;
			}

			// class
			var result = new JSONClass();
			foreach (var field in objType.GetFields(BindingFlags.Instance | BindingFlags.Public))
			{
				var attr = field.GetCustomAttributes(typeof(JsonIgnoreAttribute), true);
				if (attr.Length > 0)
					continue;

				var value = field.GetValue(obj);
				if (value != GetDefaultValue(field.FieldType))
				{
					result.Add(field.Name, GetNodeFromValue(value));
				}
			}

			foreach (var field in objType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				var attr = field.GetCustomAttributes(typeof(JsonSerializeAttribute), true);
				if (attr.Length == 0)
					continue;

				var value = field.GetValue(obj);
				if (value != GetDefaultValue(field.FieldType))
				{
					result.Add(field.Name, GetNodeFromValue(value));
				}
			}

			foreach (var property in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				// indexed property
				if (property.GetIndexParameters().Length > 0 || !property.CanRead || !property.CanWrite)
					continue;

				var attr = property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true);
				if (attr.Length > 0)
					continue;

				var value = property.GetValue(obj, null);
				if (value != GetDefaultValue(property.PropertyType))
				{
					result.Add(property.Name, GetNodeFromValue(value));
				}
			}

			result.Add(CLASS_NAME_FIELD, new JSONData(objType.Name));

			return result;
		}

		private static object GetDefaultValue(Type type)
		{
			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}

		private static bool FastStringCompare(string a, string b)
		{
			var aLength = a.Length;
			var bLength = b.Length;

			if (aLength != bLength)
			{
				return false;
			}

			for (int i = 0; i < aLength; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}

			return true;
		}

		private static List<Type> GetAllTypesByName(string className)
		{
			List<Type> result = new List<Type>();

			var typePairs = m_cachedTypesDictionary[className[0]];

			for (int i = 0; i < typePairs.Count; i++)
			{
				var typePair = typePairs[i];

				if (FastStringCompare(typePair.Key, className))
				{
					result.Add(typePair.Value);
				}
			}

			return result;
		}

		private static DateTime ParseDateTime(string s)
		{
			try
			{
				return DateTime.ParseExact(s, DATETIME_FORMAT_STRING, null);
			}
			catch
			{
				return DateTime.Parse(s);
			}
		}
	}
}