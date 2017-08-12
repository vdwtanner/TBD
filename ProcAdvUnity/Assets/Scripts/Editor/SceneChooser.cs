
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneChooser : MonoBehaviour{
	delegate void LoadSceneDelegate();
	static LoadSceneDelegate currentFunc;
	static LoadSceneDelegate previousFunc; 

	[MenuItem("Scenes/ Previous Scene &p", false, 15)]
	static void LoadPreviousScene()
	{
		if (previousFunc != null)
		{
			previousFunc();
		}
	}

	[MenuItem("Scenes/ Previous Scene &p", true, 15)]
	static bool LoadPreviousSceneValidation()
	{
		return previousFunc != null;
	}

	[MenuItem("Scenes/Game")]
	static void LoadLevel_Game()
	{
		RegisterPreviousScene(currentFunc);
		currentFunc = LoadLevel_Game;
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/Game.unity");
	}

	[MenuItem("Scenes/MainMenu")]
	static void LoadLevel_MainMenu()
	{
		RegisterPreviousScene(currentFunc);
		currentFunc = LoadLevel_MainMenu;
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/MainMenu.unity");
	}

	[MenuItem("Scenes/Test/PregenTest")]
	static void LoadLevel_TestPregenTest()
	{
		RegisterPreviousScene(currentFunc);
		currentFunc = LoadLevel_TestPregenTest;
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/Test/PregenTest.unity");
	}

	[MenuItem("Scenes/Test/TerrainTest")]
	static void LoadLevel_TestTerrainTest()
	{
		RegisterPreviousScene(currentFunc);
		currentFunc = LoadLevel_TestTerrainTest;
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/Test/TerrainTest.unity");
	}

	static void RegisterPreviousScene(LoadSceneDelegate func)
	{
		previousFunc = func;
	}
}