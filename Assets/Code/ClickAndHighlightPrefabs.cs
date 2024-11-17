using UnityEngine;
using System.Collections.Generic;

public class HighlightMultipleSceneObjects : MonoBehaviour
{
    [Header("Scene Objects (Add your objects here)")]
    public GameObject[] prefabs; // Scene objects to highlight

    [Header("Light Settings")]
    public float lightHeight = 2.0f; // Height of the light above the object
    public Color lightColor = Color.yellow; // Color of the light
    public float lightIntensity = 2.0f; // Intensity of the light

    private Dictionary<GameObject, GameObject> prefabLights = new Dictionary<GameObject, GameObject>(); // Map for objects and their lights
    private HashSet<GameObject> selectedObjects = new HashSet<GameObject>(); // Keep track of selected objects

    void Start()
    {
        foreach (var obj in prefabs)
        {
            if (obj != null)
            {
                AddHighlightLight(obj);
            }
        }
    }

    void Update()
    {
        HandleMouseClick();
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // Detect left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                // Check if the clicked object is in our managed list
                if (prefabLights.ContainsKey(clickedObject))
                {
                    if (selectedObjects.Contains(clickedObject))
                    {
                        // If already selected, deselect and turn off light
                        ToggleLight(clickedObject, false);
                        selectedObjects.Remove(clickedObject);
                    }
                    else
                    {
                        // Otherwise, select and turn on light
                        ToggleLight(clickedObject, true);
                        selectedObjects.Add(clickedObject);
                    }
                }
            }
        }
    }

    private void AddHighlightLight(GameObject obj)
    {
        if (!obj.GetComponent<Collider>())
        {
            Debug.LogWarning($"Object {obj.name} is missing a Collider component!");
        }

        // Create a light as a child of the object
        GameObject lightObject = new GameObject("HighlightLight");
        lightObject.transform.parent = obj.transform;
        lightObject.transform.localPosition = new Vector3(0, lightHeight, 0);

        // Add and configure a light component
        Light lightComponent = lightObject.AddComponent<Light>();
        lightComponent.type = LightType.Point;
        lightComponent.color = lightColor;
        lightComponent.intensity = lightIntensity;
        lightComponent.range = 5.0f;
        lightObject.SetActive(false); // Start with light turned off

        // Map the light to the object
        prefabLights[obj] = lightObject;
    }

    private void ToggleLight(GameObject obj, bool state)
    {
        if (prefabLights.ContainsKey(obj))
        {
            prefabLights[obj].SetActive(state);
        }
    }
}
