using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;

public class tracking : MonoBehaviour
{
    [Serializable]
    public class TrackingData
    {
        public float TotalTime;

        public Gaze Gaze;

        public ReactionTimes ReactionTimes;
    }

    [Serializable]
    public class Gaze
    {
        public Music Music;
        public Cars Cars;
        public Cookbooks Cookbooks;
    }

    // probably eventually want to make this a list/dictionary, not hard-coded
    [Serializable]
    public class Music
    {
        public float Rap;

        public float Reggae;

        public float EDM;

        public float Country;

        public float Oldies;
    }

    [Serializable]
    public class Cars
    {
        public float Hybrid;

        public float Pickup;
    }

    [Serializable]
    public class Cookbooks
    {
        public float Gastronomy;

        public float Spicy;
    }

    [Serializable]
    public class ReactionTimes
    {
        public float Cat;
    }

    public GameObject[] allObjects;
    public Dictionary<string, float> trackingData = new Dictionary<string, float>();
    public bool istracking = true;
    public float totalCounter = 0;
    
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

    void FixedUpdate()
    {   
        if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger) || totalCounter % 500 == 0)
        {
            if (istracking == true)
            {
                // // StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/tracking_data.txt", true);
                // StreamWriter writer = new StreamWriter("Assets/tracking_data.txt", true);

                // writer.WriteLine("Total time: " + totalCounter * Time.fixedDeltaTime);
                // if(reactionTime > 0)
                // {
                //     writer.WriteLine("Reaction time: " + reactionTime);
                // }

                // foreach (var currObject in allObjects)
                // {
                //     writer.WriteLine(currObject.name + ": " + trackingData[currObject.name]);
                // }

                // writer.Close();

                var data = new TrackingData
                {
                    TotalTime = totalCounter * Time.fixedDeltaTime,
                    Gaze = new Gaze
                    {
                        Music = new Music
                        {
                            Rap = trackingData["rap"],
                            Reggae = trackingData["reggae"],
                            EDM = trackingData["EDM"],
                            Country = trackingData["country"],
                            Oldies = trackingData["oldies"]
                        },
                        
                        Cars = new Cars
                        {
                            Hybrid = trackingData["hybrid vehicles"],
                            Pickup = trackingData["pickup trucks"]
                        },
                        
                        Cookbooks = new Cookbooks
                        {
                            Gastronomy = trackingData["molecular gastronomy"],
                            Spicy = trackingData["spicy food"]
                        }
                    },

                    ReactionTimes = new ReactionTimes
                    {
                        Cat = reactionTime
                    }
                };
                //this is truly terrible code and I'm deeply sorry but you do what you gotta do
                string json = JsonUtility.ToJson(data);
                json = json.Replace("TotalTime", "total time");
                json = json.Replace("Gaze", "gaze");
                json = json.Replace("Music", "music");
                json = json.Replace("Cars", "cars");
                json = json.Replace("Cookbooks", "cookbooks");
                json = json.Replace("ReactionTimes", "reaction times");
                json = json.Replace("Rap", "rap");
                json = json.Replace("Reggae", "reggae");
                json = json.Replace("EDM", "EDM");
                json = json.Replace("Country", "country");
                json = json.Replace("Oldies", "oldies");
                json = json.Replace("Hybrid", "hybrid vehicles");
                json = json.Replace("Pickup", "pickup trucks");
                json = json.Replace("Gastronomy", "molecular gastronomy");
                json = json.Replace("Spicy", "spicy food");
                json = json.Replace("Cat", "cat");

                File.WriteAllText("Assets/tracking_data.json", json);

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
                // Debug.DrawRay(pos, forward * 100, Color.green);
                // GameObject line = new GameObject();
                // line.transform.position = pos;
                // line.AddComponent<LineRenderer>();

                // LineRenderer lr = line.GetComponent<LineRenderer>();
                // lr.material.SetColor("_Color", Color.red);
                // lr.startWidth = 0.01f;
                // lr.endWidth = 0.01f;
                // lr.SetPosition(0, pos);
                // lr.SetPosition(1, forward * 1000);
                // GameObject.Destroy(line, 0.025f);

                // lookingAtDebug = hit.collider.gameObject;
                // string hitObjectName = lookingAtDebug.name;
                // if (trackingData.ContainsKey(hitObjectName))
                // {
                //     trackingData[hitObjectName] += Time.fixedDeltaTime;
                // }

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
                    trackingData[allObjects[min_indx].name] += Time.fixedDeltaTime;
                }
            }

            totalCounter += 1;
        }

        // check if seen the cat
        if (reactionTime < 0f && reactionTimeCounter < 8f)
        {
            RaycastHit hit;
            Vector3 pos = camera.transform.position;
            Vector3 forward = camera.transform.forward;

            if (Physics.Raycast(pos, forward, out hit, Mathf.Infinity))
            {
                localHit = hit.point;
                // Debug.DrawRay(pos, forward * 100, Color.green);
                if (Vector3.Distance(cat.transform.position, localHit) <= 1.3)
                {
                    reactionTime = reactionTimeCounter - 5.2f;  // meow starts after 5.2 seconds in audio clip; negative values (false positives, aka the user looked before the sound played) will be disregarded later
                    Debug.Log("Seen!");
                }
            }

            reactionTimeCounter += Time.fixedDeltaTime;
        }
    }
}