using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

#if UNITY_EDITOR
[ExecuteInEditMode]
[SelectionBase]
[System.Serializable]
public class PlatformController : MonoBehaviour
{
    public String baseName = "room0";
    internal Transform platformPrefab = null;
    internal Transform wallPrefab = null;
    internal Transform doorPrefab = null;
    private Vector3 lastWallDirection = Vector3.up;

    void Start()
    {
        platformPrefab = AssetDatabase.LoadAssetAtPath<Transform>("Assets/Custom Assets/Prefabs/" + "platform-current.prefab");
        wallPrefab = (Transform)AssetDatabase.LoadAssetAtPath<Transform>("Assets/Custom Assets/Prefabs/" + "wall.prefab");
        doorPrefab = (Transform)AssetDatabase.LoadAssetAtPath<Transform>("Assets/Custom Assets/Prefabs/" + "door.prefab");
    }

    public void createPlatform(Vector3 direction)
    {
        if (!this.name.Contains(baseName))
            renameObject(this.transform, "base");

        Vector3 size = GetComponent<BoxCollider2D>().bounds.size;
        Vector3 newPosition = this.transform.position;
        if (this.name.Contains("wall"))
        {
            newPosition += lastWallDirection * (size.y / 2);
            newPosition += Vector3.right * (platformPrefab.GetComponent<BoxCollider2D>().bounds.size.x / 2);
        }
        else
            newPosition += direction * size.x;
        Transform obj = (Transform)PrefabUtility.InstantiatePrefab(platformPrefab);
        obj.position = newPosition;
        initializeProperties(obj, "platform");
    }

    public void createWall(Vector3 direction)
    {
        Vector3 floorSize = GetComponent<BoxCollider2D>().bounds.size;
        Vector3 wallSize = wallPrefab.FindChild("texture").GetComponent<MeshRenderer>().bounds.size;
        Vector3 newPosition = this.transform.position;
        if (direction == Vector3.up || direction == Vector3.down)
        {
            lastWallDirection = direction;
            newPosition += direction * wallSize.y;
        }
        else if (direction != Vector3.zero)
            newPosition += Vector3.up * (wallSize.y / 2);
        if (direction != Vector3.zero)
            newPosition += direction * ((floorSize.x / 2) - (wallSize.x / 2));
        Transform obj = (Transform)PrefabUtility.InstantiatePrefab(wallPrefab);
        obj.position = newPosition;
        initializeProperties(obj, "wall");
    }

    internal void createDoor(Vector3 direction)
    {
        Vector3 floorSize = GetComponent<BoxCollider2D>().bounds.size;
        Vector3 doorSize = doorPrefab.FindChild("backofdoor").GetComponent<MeshRenderer>().bounds.size;
        Vector3 newPosition = this.transform.position;
        newPosition += Vector3.up * (doorSize.y / 2);
        newPosition += direction * ((floorSize.x / 2) - (doorSize.x / 2));
        Transform obj = (Transform)PrefabUtility.InstantiatePrefab(doorPrefab);
        obj.position = newPosition;
        initializeProperties(obj, "door");
    }

    private void initializeProperties(Transform obj, String type)
    {
        Undo.RegisterCreatedObjectUndo(obj.gameObject, "Create " + obj.name);
        obj.transform.parent = transform.parent;
        renameObject(obj, type);

        PlatformController controller = obj.gameObject.GetComponent<PlatformController>();
        controller.platformPrefab = platformPrefab;
        controller.wallPrefab = wallPrefab;
        controller.doorPrefab = doorPrefab;

        Selection.objects = new UnityEngine.Object[] { obj.gameObject };
    }

    private void renameObject(Transform obj, string type)
    {
        obj.gameObject.name = baseName + "-" + type + " 0x" + obj.GetHashCode();
    }
}
#endif

//Create Button to allow the Update while in Editor
#if UNITY_EDITOR
[CustomEditor(typeof(PlatformController))]
[CanEditMultipleObjects]
public class PlatformControl : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlatformController controller = (PlatformController)target;

        GUILayout.BeginHorizontal("box");
        controller.platformPrefab = (Transform)EditorGUILayout.ObjectField("Create Platform", controller.platformPrefab, typeof(Transform), true);
        if (controller.platformPrefab != null)
        {
            if (GUILayout.Button("Left"))
                controller.createPlatform(Vector3.left);
            if (GUILayout.Button("Right"))
                controller.createPlatform(Vector3.right);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        controller.wallPrefab = (Transform)EditorGUILayout.ObjectField("Create Wall", controller.wallPrefab, typeof(Transform), true);
        if (controller.wallPrefab != null)
        {
            if (controller.name.Contains("wall"))
            {
                if (GUILayout.Button("Up"))
                    controller.createWall(Vector3.up);
                if (GUILayout.Button("Down"))
                    controller.createWall(Vector3.down);
            }
            else
            {
                if (GUILayout.Button("Left"))
                    controller.createWall(Vector3.left);
                if (GUILayout.Button("Right"))
                    controller.createWall(Vector3.right);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        controller.doorPrefab = (Transform)EditorGUILayout.ObjectField("Create Door", controller.doorPrefab, typeof(Transform), true);
        if (controller.doorPrefab != null)
        {
            if (GUILayout.Button("Left"))
                controller.createDoor(Vector3.left);
            if (GUILayout.Button("Right"))
                controller.createDoor(Vector3.right);
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Toggle shadow caster"))
        {
            Debug.Log("TOGGLE");
            MeshRenderer renderer = controller.transform.FindChild("shadow-caster").GetComponent<MeshRenderer>();
            if (renderer.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.On)
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            else
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }

        if (GUILayout.Button("Randomize platform layering"))
            foreach (GameObject obj in Selection.objects)
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, UnityEngine.Random.Range(0, -0.01f));
    }
}
#endif
