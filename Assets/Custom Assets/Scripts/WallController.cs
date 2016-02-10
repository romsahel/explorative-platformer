using UnityEngine;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

#if UNITY_EDITOR
[ExecuteInEditMode, SelectionBase, System.Serializable]
public class WallController : MonoBehaviour
{
    internal Transform[] prefabs;
    public String baseName = "room0";
    private System.Random random = new System.Random();


    void Awake()
    {
        String pathToPrefabs = "Assets/Custom Assets/Prefabs/Backgrounds/";
        prefabs = new Transform[]
		{
			(Transform)AssetDatabase.LoadAssetAtPath(pathToPrefabs + "wall1.prefab", typeof(Transform)),
			(Transform)AssetDatabase.LoadAssetAtPath(pathToPrefabs + "wall2.prefab", typeof(Transform)),
			(Transform)AssetDatabase.LoadAssetAtPath(pathToPrefabs + "window1.prefab", typeof(Transform)),
			(Transform)AssetDatabase.LoadAssetAtPath(pathToPrefabs + "window2.prefab", typeof(Transform)),
			(Transform)AssetDatabase.LoadAssetAtPath(pathToPrefabs + "window3.prefab", typeof(Transform)),
			(Transform)AssetDatabase.LoadAssetAtPath(pathToPrefabs + "window4.prefab", typeof(Transform)),
			(Transform)AssetDatabase.LoadAssetAtPath(pathToPrefabs + "window5.prefab", typeof(Transform)),
			(Transform)AssetDatabase.LoadAssetAtPath(pathToPrefabs + "window6.prefab", typeof(Transform)),
			(Transform)AssetDatabase.LoadAssetAtPath(pathToPrefabs + "window7.prefab", typeof(Transform)),
    };
    }

    internal Transform createRandomBackground(Vector3 rightLeftDirection, Vector3 upDownDirection, int min, int max)
    {
        Vector3 size = GetComponent<MeshRenderer>().bounds.size;
        Vector3 newPosition = this.transform.position;
        newPosition += rightLeftDirection * size.x;
        newPosition += upDownDirection * size.y;
        int index = random.Next(min, max);
        return createObject("bg-wall", prefabs[index], newPosition);
    }

    internal void changeCurrentBackground(Vector3 rightLeftDirection, Vector3 upDownDirection, int min, int max)
    {
        createRandomBackground(Vector3.zero, Vector3.zero, min, max);
        DestroyImmediate(this.gameObject);
    }

    private Transform createObject(String type, Transform prefab, Vector3 position)
    {
        Transform obj = (Transform)PrefabUtility.InstantiatePrefab(prefab);
        obj.position = position;
        Undo.RegisterCreatedObjectUndo(obj.gameObject, "Create " + obj.name);
        obj.transform.parent = transform.parent;
        obj.GetComponent<WallController>().baseName = baseName;
        renameObject(obj, type);

        Selection.objects = new UnityEngine.Object[] { obj.gameObject };
        return obj;
    }

    private void renameObject(Transform obj, string type)
    {
        obj.gameObject.name = baseName + "-" + type + " 0x" + obj.GetHashCode();
    }

    public void addFlavorToWall(bool randomize, int chances)
    {
        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);
        if (randomize && UnityEngine.Random.Range(0, 100) > chances)
            return;

        const string path = "Assets/Custom Assets/Materials/Backgrounds/Flavors/flavor";
        var obj = createObject("flavor", prefabs[0], Vector3.zero);
        obj.parent = this.transform;
        //obj.localPosition = Vector3.up * 0.01f;
        obj.localPosition = Vector3.zero;
        int index = (1 + random.Next(10 - 1));
        string mat = path + index + ".mat";
        obj.GetComponent<MeshRenderer>().material = AssetDatabase.LoadAssetAtPath<Material>(mat);
        DestroyImmediate(obj.GetComponent<WallController>());
        Selection.objects = new UnityEngine.Object[] { this.gameObject };
    }
}
#endif

//Create Button to allow the Update while in Editor
#if UNITY_EDITOR
[CustomEditor(typeof(WallController))]
[CanEditMultipleObjects]
public class WallControl : Editor
{
    private static int nbWallTypes = 2;
    private static int nbWindowTypes = 7;
    private int numInstancesToCreate = 0;

    static void createController(WallController controller, String name, int min, int max)
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Create " + name);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Left"))
            controller.createRandomBackground(Vector3.left, Vector3.zero, min, max);
        if (GUILayout.Button("Right"))
            controller.createRandomBackground(Vector3.right, Vector3.zero, min, max);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Down"))
            controller.createRandomBackground(Vector3.zero, Vector3.down, min, max);
        if (GUILayout.Button("Up"))
            controller.createRandomBackground(Vector3.zero, Vector3.up, min, max);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    private void createSeriesOfWalls(WallController controller, Vector3 horizontal, Vector3 vertical)
    {
        WallController current = controller;
        for (int i = 0; i < numInstancesToCreate; i++)
            current = (current.createRandomBackground(horizontal, vertical, 0, nbWallTypes)).GetComponent<WallController>();

    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WallController controller = (WallController)target;

        createController(controller, "Wall", 0, nbWallTypes);
        createController(controller, "Window", nbWallTypes, nbWindowTypes);
        createController(controller, "Any Background", 0, nbWallTypes + nbWindowTypes);

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Change texture");
        if (GUILayout.Button("Wall"))
            controller.changeCurrentBackground(Vector3.left, Vector3.zero, 0, nbWallTypes);
        if (GUILayout.Button("Window"))
            controller.changeCurrentBackground(Vector3.right, Vector3.zero, nbWallTypes, nbWindowTypes);
        if (GUILayout.Button("Random"))
            controller.changeCurrentBackground(Vector3.left, Vector3.zero, 0, nbWallTypes + nbWindowTypes);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Create backgrounds");
        numInstancesToCreate = (int)GUILayout.HorizontalSlider(numInstancesToCreate, 0, 100);
        if (GUILayout.Button("Left"))
            foreach (GameObject obj in Selection.objects)
                createSeriesOfWalls(obj.GetComponent<WallController>(), Vector3.left, Vector3.zero);
        if (GUILayout.Button("Down"))
            foreach (GameObject obj in Selection.objects)
                createSeriesOfWalls(obj.GetComponent<WallController>(), Vector3.zero, Vector3.down);
        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal("box");

        GUILayout.Label("Flavors");
        if (GUILayout.Button("Add"))
            foreach (GameObject obj in Selection.objects)
                obj.GetComponent<WallController>().addFlavorToWall(false, -1);
        if (GUILayout.Button("Randomize"))
        {
            var selection = Selection.objects;
            foreach (GameObject obj in Selection.objects)
                obj.GetComponent<WallController>().addFlavorToWall(true, 10);
            Selection.objects = selection;
        }
        if (GUILayout.Button("Remove"))
        {
            var selection = Selection.objects;
            foreach (GameObject obj in Selection.objects)
                obj.GetComponent<WallController>().addFlavorToWall(true, -1);
            Selection.objects = selection;
        }
        GUILayout.EndHorizontal();

    }
}
#endif