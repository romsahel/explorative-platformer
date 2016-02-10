using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Door : MonoBehaviour
{
    public enum Type
    {
        GOLD = 0,
        RED,
        EXIT
    }

    public bool isLocked;
    public Type doorType;

    private bool _isOpen;
    private Light[] _lightsArray;
    private static Texture[] goldTextures = null, redTextures = null, exitTextures = null;
    private Transform parentRoom;

    void Awake()
    {
        _isOpen = false;

        parentRoom = transform.parent;
        Transform lights = getLights(parentRoom);
        if (lights == null)
            if (parentRoom != null)
                lights = getLights(parentRoom.parent);
        if (lights != null)
        {
            _lightsArray = new Light[lights.childCount];
            int i = 0;
            foreach (Transform child in lights)
                _lightsArray[i++] = child.GetComponent<Light>();
        }

        if (goldTextures == null)
        {
            goldTextures = new Texture[5];
            redTextures = new Texture[5];
            exitTextures = new Texture[5];
            for (int i = 1; i < 6; i++)
            {
                goldTextures[i - 1] = loadFrameTexture("goldfade", i);
                redTextures[i - 1] = loadFrameTexture("redfade", i);
                exitTextures[i - 1] = loadFrameTexture("redfade", i);
            }
            //exitTextures[6 - 1] = loadFrameTexture("exitfade", 6);
            //exitTextures[7 - 1] = loadFrameTexture("exitfade", 7);
        }
    }

    private Transform getLights(Transform parent)
    {
        if (parent == null)
            return null;
        return parent.FindChild("Lights");
    }

    public void close()
    {
        _isOpen = false;
    }

    public Transform open()
    {
        if (_lightsArray != null)
            foreach (Light light in _lightsArray)
            {
                if (!light.enabled)
                    StartCoroutine(FadeOn(light, 0f, light.intensity));
                light.enabled = true;
            }
        if (_isOpen)
            return parentRoom;

        _isOpen = true;
        StartCoroutine(animatePosition(transform.FindChild("shadow-caster").transform, transform.position, transform.position + Vector3.forward * 0.65f));

        
        if (isLocked)
        {
            string name = (doorType == Type.GOLD) ? "gold-texture" : ((doorType == Type.RED) ? "red-texture" : "exit-texture");
            Texture[] textures = (doorType == Type.GOLD) ? goldTextures : ((doorType == Type.RED) ? redTextures : exitTextures);
            StartCoroutine(animateTexture(transform.FindChild(name), textures));
        }
        isLocked = false;

        this.gameObject.layer = LayerMask.NameToLayer("OpenedDoors");
        return parentRoom;
    }

    IEnumerator animateTexture(Transform obj, Texture[] textures)
    {
        int counter = 1;
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        const float speed = 0.15f;

        while (counter < 6)
        {
            renderer.material.mainTexture = textures[counter - 1];
            yield return new WaitForSeconds(speed);
            counter++;
        }
        obj.gameObject.SetActive(false);
    }

    Texture loadFrameTexture(string name, int counter)
    {
        const string path = "floor,door++/Portal/";
        return Resources.Load<Texture>(path + name + "/" + name + counter);
    }

    public Type getDoorType()
    {
        return doorType;
    }

    public bool isOpen()
    {
        return _isOpen;
    }


    IEnumerator FadeOn(Light light, float startIntensity, float targetIntensity)
    {
        float progress = 0.0f;
        const float speed = 2f;
        light.intensity = startIntensity;
        //ready = false;
        while (progress < 1.0f)
        {
            light.intensity = Mathf.Lerp(startIntensity, targetIntensity, Mathf.SmoothStep(0, 1, progress));
            yield return new WaitForEndOfFrame();
            progress += Time.deltaTime * speed;
        }
        //ready = true;

        light.intensity = targetIntensity;
    }

    IEnumerator animatePosition(Transform transform, Vector3 startPos, Vector3 endPos)
    {
        float progress = 0.0f;
        const float speed = 1f;

        transform.position = startPos;
        while (progress < 1.0f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, progress));

            yield return new WaitForEndOfFrame();
            progress += Time.deltaTime * speed;
        }
        transform.position = endPos;
    }
    public static void changeState(Door controller, Door.Type type, bool locked)
    {
        controller.doorType = type;
        controller.isLocked = locked;

        controller.transform.FindChild("gold-texture").gameObject.SetActive(locked && type == Door.Type.GOLD);
        controller.transform.FindChild("red-texture").gameObject.SetActive(locked && type == Door.Type.RED);
    }
}


//Create Button to allow the Update while in Editor

#if UNITY_EDITOR
[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        Door controller = (Door)target;

        Door.changeState(controller, controller.doorType, EditorGUILayout.Toggle("Is Locked", controller.isLocked));

        if (controller.isLocked)
            Door.changeState(controller, (Door.Type)EditorGUILayout.EnumPopup("Door Type", controller.getDoorType()), controller.isLocked);

        if (GUI.changed)
            EditorUtility.SetDirty(controller);
    }
}
#endif