using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class tracking : MonoBehaviour
{
    public GameObject[] allObjects;
    public Dictionary<string, float> trackingData = new Dictionary<string, float>();
    public bool istracking = true;
    public float totalTime = 0;
    
    public GameObject camera;
    public GameObject lookingAtDebug;
    public GameObject cat;
    
    public Vector3 localHit = Vector3.zero;

    public float reactionTime = -1;
    public float reactionTimeCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        allObjects = GameObject.FindGameObjectsWithTag("tracking");
        foreach (var currObject in allObjects)
        {
            trackingData[currObject.name] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
        {
            if (istracking == true)
            {
                // save the data
                string json = JsonUtility.ToJson(trackingData);

                StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/tracking_data.txt", true);
                // StreamWriter writer = new StreamWriter("Assets/tracking_data.txt", true);

                writer.WriteLine("Total time: " + totalTime);
                writer.WriteLine("Reaction time: " + reactionTime);

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
            //istracking = !istracking;
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
                //Debug.DrawRay(pos, forward * 100, Color.green);
                lookingAtDebug = hit.collider.gameObject;
                string hitObjectName = lookingAtDebug.name;
                if (trackingData.ContainsKey(hitObjectName))
                {
                    trackingData[hitObjectName] += 1;
                }
            }
            totalTime += 1;
        }

        // check if seen the cat
        if (reactionTime == -1)
        {
            RaycastHit hit;
            Vector3 pos = camera.transform.position;
            Vector3 forward = camera.transform.forward;

            if (Physics.Raycast(pos, forward, out hit, Mathf.Infinity))
            {
                localHit = hit.point;
                //Debug.DrawRay(pos, forward * 100, Color.green);
                lookingAtDebug = hit.collider.gameObject;
                string hitObjectName = lookingAtDebug.name;
                if (Vector3.Distance(cat.transform.position, localHit) <= 1.2)
                {
                    reactionTime = reactionTimeCounter;
                    Debug.Log("Seen!");
                }
            }

            reactionTimeCounter += 1;
        }

        totalTime += 1;
    }
}