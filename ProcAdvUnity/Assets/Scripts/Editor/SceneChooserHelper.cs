using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SceneChooserHelper : MonoBehaviour {
	static string path = "";
	static string sceneChooserScriptPath = "";
	static TextAsset text;

	[MenuItem("Scenes/Set Scene Directory", false, 1)]
	static void SetSceneDir()
	{
		string newPath = EditorUtility.OpenFolderPanel("Select Root path for scenes", path, "");
		if(newPath != "")
		{
			path = newPath;
			UpdateListing();
		}
	}

	[MenuItem("Scenes/Update Scene Listing", false, 1)]
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
		output += "\n\tdelegate void LoadSceneDelegate();\n\tstatic LoadSceneDelegate currentFunc;\n\tstatic LoadSceneDelegate previousFunc; ";
		output += "\n\n\t[MenuItem(\"Scenes/ Previous Scene &p\", false, 15)]\n\tstatic void LoadPreviousScene()\n\t{\n\t\tif (previousFunc != null)\n\t\t{\n\t\t\tpreviousFunc();\n\t\t}\n\t}";
		output += "\n\n\t[MenuItem(\"Scenes/ Previous Scene &p\", true, 15)]\n\tstatic bool LoadPreviousSceneValidation()\n\t{\n\t\treturn previousFunc != null;\n\t}";
		foreach (string file in allFiles)
		{
			string menuItemName = file.Replace(path, "").Replace("\\", "/");
			menuItemName = menuItemName.Substring(1,menuItemName.Length - 7);
			string funcName = menuItemName.Replace("/", "");
			string pathToScene = file.Replace(Application.dataPath, "").Replace("\\", "/");
			output += "\n\n\t[MenuItem(\"Scenes/"+ menuItemName + "\")]";
			output += "\n\tstatic void LoadLevel_" + funcName + "()\n\t{";
			output += "\n\t\tRegisterPreviousScene(currentFunc);";
			output += "\n\t\tcurrentFunc = LoadLevel_" + funcName + ";";
			output += "\n\t\tEditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();";
			output += "\n\t\tEditorSceneManager.OpenScene(Application.dataPath + \"" + pathToScene + "\");";
			output += "\n\t}";
		}
		output += "\n\n\tstatic void RegisterPreviousScene(LoadSceneDelegate func)\n\t{\n\t\tpreviousFunc = func;\n\t}";
		output += "\n}";
		System.IO.File.WriteAllText(sceneChooserScriptPath, output);
		AssetDatabase.Refresh();
		EditorUtility.DisplayDialog("U CAN HAS SCENE SELECSHUN", "Processed " + allFiles.Length + " scenes.", "Thank you, benevolent script!");
	}
}
