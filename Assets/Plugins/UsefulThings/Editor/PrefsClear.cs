using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PrefsClear : Editor
{

	[MenuItem("Prefs/ClearAll")]
	public static void ClearAll()
	{
		ClearPrefs();
		ClearSaves();
	}

	[MenuItem("Prefs/ClearPrefs")]
	public static void ClearPrefs()
	{
		PlayerPrefs.DeleteAll();
	}

	[MenuItem("Prefs/ClearSaves")]
	public static void ClearSaves()
	{
		var path = Path.Combine(Application.persistentDataPath, "Saves");
		if (Directory.Exists(path))
		{
			Directory.Delete(path, true);
		}
	}
}
