using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using System.IO;

public class SceneChooserHelper : MonoBehaviour {
	static string path = "";
	static string sceneChooserScriptPath = "";
	static TextAsset text;

	[MenuItem("Utils/Scenes/Set Scene Directory")]
	static void SetSceneDir()
	{
		string newPath = EditorUtility.OpenFolderPanel("Select Root path for scenes", path, "");
		if(newPath != "")
		{
			path = newPath;
			bool update = EditorUtility.DisplayDialog("Update Scene Chooser", "Would you like to update Scene list from\n" + path, "kthxbai", "plzno");
			if (update)
			{
				UpdateListing();
			}
		}
	}

	[MenuItem("Utils/Scenes/Update Scene Listing")]
	static void UpdateListing()
	{
		sceneChooserScriptPath = Application.dataPath + "/Scripts/Editor/SceneChooser.cs";
		EditorUtility.DisplayDialog("Did things", sceneChooserScriptPath, "qule");
		string[] allFiles = System.IO.Directory.GetFiles(path, "*.unity", System.IO.SearchOption.AllDirectories);
		string output = "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEditor;\nusing UnityEditor.SceneManagement;\n\npublic class SceneChooser : MonoBehaviour{";
		foreach(string file in allFiles)
		{
			string menuItemName = file.Replace(path, "").Replace("\\", "/");
			menuItemName = menuItemName.Substring(1,menuItemName.Length - 7);
			string funcName = menuItemName.Replace("/", "");
			string pathToScene = file.Replace(Application.dataPath, "").Replace("\\", "/");
			output += "\n\n\t[MenuItem(\"Scenes/"+ menuItemName + "\")]";
			output += "\n\tstatic void LoadLevel_" + funcName + "()\n\t{";
			output += "\n\t\tEditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();";
			output += "\n\t\tEditorSceneManager.OpenScene(Application.dataPath + \"" + pathToScene + "\");";
			output += "\n\t}";
		}
		output += "\n}";
		System.IO.File.WriteAllText(sceneChooserScriptPath, output);
		AssetDatabase.Refresh();
	}
}
