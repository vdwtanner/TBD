
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneChooser : MonoBehaviour{

	[MenuItem("Scenes/Game")]
	static void LoadLevel_Game()
	{
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/Game.unity");
	}

	[MenuItem("Scenes/MainMenu")]
	static void LoadLevel_MainMenu()
	{
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/MainMenu.unity");
	}

	[MenuItem("Scenes/Test/PregenTest")]
	static void LoadLevel_TestPregenTest()
	{
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/Test/PregenTest.unity");
	}

	[MenuItem("Scenes/Test/TerrainTest")]
	static void LoadLevel_TestTerrainTest()
	{
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/Test/TerrainTest.unity");
	}
}