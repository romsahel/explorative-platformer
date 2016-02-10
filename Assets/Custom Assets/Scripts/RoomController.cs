using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

[ExecuteInEditMode]
public class RoomController : MonoBehaviour
{

#if UNITY_EDITOR
    internal void toggleCategory(Transform category)
    {
        foreach (Transform t in category)
        {
            Light light = t.GetComponent<Light>();
            if (light != null)
                light.enabled = !light.enabled;
            else
                t.gameObject.SetActive(!t.gameObject.activeSelf);
        }
    }

    internal void selectCategory(Transform category)
    {
        UnityEngine.Object[] selection = new UnityEngine.Object[category.childCount];
        int i = 0;
        foreach (Transform t in category)
            selection[i++] = t.gameObject;
        Selection.objects = selection;
    }
#endif
}

//Create Button to allow the Update while in Editor
#if UNITY_EDITOR
[CustomEditor(typeof(RoomController))]
public class ControlRoom : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomController controller = (RoomController)target;

        foreach (Transform t in controller.transform)
        {
            GUILayout.Label(t.name);
            if (GUILayout.Button("Toggle " + t.name))
                controller.toggleCategory(t);
            if (GUILayout.Button("Select " + t.name))
                controller.selectCategory(t);
        }
    }
}
#endif