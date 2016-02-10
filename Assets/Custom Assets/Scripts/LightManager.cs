using UnityEngine;
using System.Collections;

public class LightManager : MonoBehaviour
{
    public Collector _collector;
    public int secondsBeforeLightsOff = 120;
    public int flickeringFrequency = 30;
    private float timer = 0;
    private int previous = 0;

    // Use this for initialization
    void Start()
    {
        timer = 0;
        previous = 0;

        var rooms = GameObject.FindObjectsOfType<RoomController>();
        foreach (RoomController r in rooms)
        {
            Light[] lights = r.gameObject.GetComponentsInChildren<Light>();
            foreach (Light l in lights)
                l.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > secondsBeforeLightsOff)
        {
            flickerAllLights(true);
            timer = 0;
            previous = (int)(timer / flickeringFrequency);
        }
        else if (flickeringFrequency != -1)
        {
            if ((int)(timer / flickeringFrequency) != previous)
            {
                flickerAllLights(false);
                previous = (int)(timer / flickeringFrequency);
            }
        }
    }

    private void flickerAllLights(bool offAtTheEnd)
    {
        Light[] allLights = FindObjectsOfType<Light>();
        float[] flickerSteps = generateFlickerSteps();
        foreach (Light l in allLights)
        {
            if (!l.enabled)
                continue;
            RoomController r = null;
            if (l.transform.parent != null && l.transform.parent.parent != null)
                r = l.transform.parent.parent.GetComponent<RoomController>();

            StartCoroutine(flicker(l, flickerSteps, offAtTheEnd, r));
        }
    }

    float[] generateFlickerSteps()
    {
        int numberOfSteps = Random.Range(3, 10);
        float[] result = new float[numberOfSteps * 2];
        var timeIndex = result.Length / 2;
        for (int i = 0; i < timeIndex; i++)
        {
            int type = Random.Range(0, 4);
            switch (type)
            {
                // short off
                case 0:
                    result[i] = Random.Range(1f / 4f, 1f / 2f);
                    result[timeIndex + i] = (Random.Range(1, 2) / 10f);
                    break;
                // long off
                case 1:
                    result[i] = Random.Range(1f / 4, 1f / 2);
                    result[timeIndex + i] = (Random.Range(1, 2) / 4f);
                    break;
                // short on
                case 2:
                    result[i] = Random.Range(1f - 0.2f, 1f + 1f);
                    result[timeIndex + i] = (Random.Range(1, 2) / 10f);
                    break;
                // long on
                case 3:
                    result[i] = Random.Range(1f - 0.2f, 1f + 1f);
                    result[timeIndex + i] = (Random.Range(1, 2) / 4f);
                    break;
            }
        }
        return result;
    }

    IEnumerator flicker(Light light, float[] steps, bool offAtTheEnd, RoomController room)
    {
        Transform currentRoom = _collector.currentRoom;
        float originalIntensity = light.intensity;
        int timeIndex = steps.Length / 2;
        for (int i = 0; i < timeIndex; i++)
        {
            light.intensity = steps[i];
            yield return new WaitForSeconds(steps[i + timeIndex]);
        }

        light.intensity = originalIntensity;
        if (offAtTheEnd)
            if (room != null && room.transform != currentRoom)
                light.enabled = false;
    }
}
