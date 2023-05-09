using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class tracking : MonoBehaviour
{
    public GameObject[] allObjects;
    public Dictionary<string, float> trackingData = new Dictionary<string, float>();
    public bool istracking = false;
    public float totalTime = 0;

    private GameObject playerObj = null;
    public GameObject camera;
    public GameObject cat;

    public Vector3[] catLocations;

    public int catLocationInd = 0;
    public GameObject lookingAtDebug;
    

    public Vector3 localHit = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        allObjects = GameObject.FindGameObjectsWithTag("tracking");
        foreach (var currObject in allObjects)
        {
            trackingData[currObject.name] = 0;
        }

        playerObj = GameObject.Find("First Person Player");
        cat.transform.position = catLocations[catLocationInd];
        catLocations = new[]
        {
            new Vector3(-1.005f, 6.844f, -3.172f), // above TV position
            new Vector3(2.658f, 5.468f, 6.473f), // kitchen counter position
            new Vector3(-20.447f, 5.295f, -4.516f) // bedroom pillow 
        };
    }

    // Update is called once per frame
    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (istracking == true)
            {
                // save the data
                string json = JsonUtility.ToJson(trackingData);

                StreamWriter writer = new StreamWriter("Assets/tracking_data.txt", true);

                writer.WriteLine("Total time: " + totalTime);

                foreach (var currObject in allObjects)
                {
                    writer.WriteLine(currObject.name + ": " + trackingData[currObject.name]);
                }

                writer.Close();
                Debug.Log("Saved Data!");
            }
            else
            {
                Debug.Log("Start Tracking!");
            }

            istracking = !istracking;
        }

        if (istracking == true)
        {
            // Cast a ray
            RaycastHit hit;
            Vector3 pos = camera.transform.position;
            Vector3 forward = camera.transform.forward;

            if (Physics.Raycast(pos, forward, out hit, Mathf.Infinity))
            {
                localHit = hit.point;
                Debug.DrawRay(pos, forward * 100, Color.green);
                lookingAtDebug = hit.collider.gameObject;
                string hitObjectName = lookingAtDebug.name;
                if (trackingData.ContainsKey(hitObjectName))
                {
                    trackingData[hitObjectName] += 1;
                }

                if (hitObjectName == "Cat Lite" && Input.GetMouseButtonDown(0))
                {
                    catLocationInd = (catLocationInd + 1) % catLocations.Length;
                    cat.transform.position = catLocations[catLocationInd];
                }
            }

            totalTime += 1;
        }
    }
}