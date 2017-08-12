using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
			UpdateListing();
		}
	}

	[MenuItem("Utils/Scenes/Update Scene Listing")]
	static void UpdateListing()
	{
		if(path == "")
		{
			if(EditorUtility.DisplayDialog("NEEDZ PATH", "No path to scenes found. Choose one now?", "Sure", "Nah"))
			{
				SetSceneDir();
			}
			return;
		}
		sceneChooserScriptPath = Application.dataPath + "/Scripts/Editor/SceneChooser.cs";
		string[] allFiles = System.IO.Directory.GetFiles(path, "*.unity", System.IO.SearchOption.AllDirectories);
		string output = "\nusing UnityEngine;\nusing UnityEditor;\nusing UnityEditor.SceneManagement;\n\npublic class SceneChooser : MonoBehaviour{";
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
		EditorUtility.DisplayDialog("U CAN HAS SCENE SELECSHUN", "All Done!", "Thank you, benevolent script!");
	}
}
