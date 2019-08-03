using UnityEngine;
using System.Collections;

public class FPSDisplayScript : MonoBehaviour
{
    float deltaTime = 0.0f;
    public float simulationTime = 0.0f;
    int numberOfEmployees = 0;
    int numberOfActiveShooters = 1;
    bool initiated = false;
    public float MaxSimulationTime = 15f * 60f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        simulationTime += Time.deltaTime;
        if(simulationTime > 2f && !initiated)
        {
            numberOfEmployees = GameObject.FindGameObjectsWithTag("Employee").Length;
            initiated = true;
            UpdateParams();
        }

        if(simulationTime >= MaxSimulationTime && MaxSimulationTime > 0)
        {
            simulationTime = 0;
            GetComponent<SimulationManager>().SendMessage("ResetSimulationRequest");
        }
    }

    void OnGUI()
    {
        if(GetComponent<Menu>().MenuOpened)
        {
            return;
        }
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 50);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);

        rect = new Rect(0, rect.size.y, w, h * 2 / 50);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        text = string.Format("Time scale: {0}x", Time.timeScale);
        GUI.Label(rect, text, style);

        rect = new Rect(0, rect.size.y*2, w, h * 2 / 50);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        text = string.Format("Time elapsed: {0:0.00} sec", simulationTime);
        GUI.Label(rect, text, style);

        rect = new Rect(0, rect.size.y * 3, w, h * 2 / 50);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        text = string.Format("All employees: {0} ", numberOfEmployees);
        GUI.Label(rect, text, style);
    }

    public void UpdateParams()
    {
        ParameterSetter setter = GameObject.FindGameObjectWithTag("ParameterSetter").GetComponent<ParameterSetter>();
        MaxSimulationTime = setter.LawEnforcementArrival * 60f;
    }
}

