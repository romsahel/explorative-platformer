using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ColliderToFit : MonoBehaviour
{

    [MenuItem("My Tools/Collider/Fit to Children")]
    static void FitToChildren()
    {
        foreach (GameObject rootGameObject in Selection.gameObjects)
        {
            if (!(rootGameObject.GetComponent<Collider>() is BoxCollider))
                continue;

            bool hasBounds = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            for (int i = 0; i < rootGameObject.transform.childCount; ++i)
            {
                Renderer childRenderer = rootGameObject.transform.GetChild(i).GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    if (hasBounds)
                    {
                        bounds.Encapsulate(childRenderer.bounds);
                    }
                    else
                    {
                        bounds = childRenderer.bounds;
                        hasBounds = true;
                    }
                }
            }

            BoxCollider collider = (BoxCollider)rootGameObject.GetComponent<Collider>();
            collider.center = bounds.center - rootGameObject.transform.position;
            collider.size = bounds.size;
        }
    }

    [MenuItem("My Tools/Randomize platform layering")]
    static void RandomizePlatformLayering()
    {
        foreach (GameObject sprite in Selection.gameObjects)
        {
            sprite.GetComponent<SpriteRenderer>().sortingOrder = sprite.GetHashCode();
        }
    }

    [MenuItem("My Tools/Replace Old Prefab")]
    static void ReplaceOldPrefab()
    {
        foreach (GameObject sprite in Selection.gameObjects)
        {
            sprite.GetComponent<PlatformController>().createPlatform(Vector3.zero);
            DestroyImmediate(sprite.gameObject);
        }
    }
    [MenuItem("My Tools/Replace Old Wall Prefab")]
    static void ReplaceWallOldPrefab()
    {
        foreach (GameObject sprite in Selection.gameObjects)
        {
            if (sprite.transform.FindChild("texture"))
                if (sprite.transform.FindChild("texture").GetComponent<SpriteRenderer>() != null)
                {
                    sprite.GetComponent<PlatformController>().createWall(Vector3.zero);
                    DestroyImmediate(sprite.gameObject);
                }
        }
    }

    [MenuItem("My Tools/Select Old Prefab")]
    static void SelectOldPrefab()
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject sprite in Selection.gameObjects)
        {
            if (sprite.transform.FindChild("2d-texture").GetComponent<SpriteRenderer>() != null)
            {
                list.Add(sprite);
            }
        }
        Selection.objects = list.ToArray();
    }
    [MenuItem("My Tools/Select Old Wall Prefab")]
    static void SelectOldWallPrefab()
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject sprite in Selection.gameObjects)
        {
            if (sprite.transform.FindChild("texture") != null)
                if (sprite.transform.FindChild("texture").GetComponent<SpriteRenderer>() != null)
                {
                    list.Add(sprite);
                }
        }
        Selection.objects = list.ToArray();
    }

}