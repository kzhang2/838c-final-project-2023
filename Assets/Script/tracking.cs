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
            Vector3 pos = playerObj.transform.position;
            Vector3 forward = playerObj.transform.forward;

            if (Physics.Raycast(pos, forward, out hit, Mathf.Infinity))
            {
                localHit = hit.point;
                Debug.DrawRay(pos, forward * 100, Color.green);
            }

            // get closest hit obj
            float min_dist = 99999999;
            int min_indx = 0;

            for (int i = 0; i < allObjects.Length; i++)
            {
                float dist = Vector3.Distance(allObjects[i].transform.position, localHit);
                if (dist < min_dist)
                {
                    min_dist = dist;
                    min_indx = i;
                }
            }
            
            if (min_dist <= 1.3)
            {
                trackingData[allObjects[min_indx].name] += 1;
            }

            Debug.Log(min_dist);

            totalTime += 1;
        }
    }
}