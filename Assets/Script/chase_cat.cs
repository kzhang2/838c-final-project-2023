using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chase_cat : MonoBehaviour
{
    private float delay = 0.025f;
    public Vector3 localHit = Vector3.zero;
    public GameObject pointingAtDebug;
    public int catLocationInd = -1;  // start at a special location near user
    public Vector3[] catLocations;
    public GameObject cat;
    // Start is called before the first frame update
    void Start()
    {
        catLocations = new[]
        {
            new Vector3(-1.005f, 6.844f, -3.172f), // above TV position
            new Vector3(2.658f, 5.468f, 6.473f), // kitchen counter position
            new Vector3(-20.447f, 5.295f, -4.516f) // bedroom pillow 
        };
        cat.transform.position = new Vector3(-5.242f, 4.154f, -0.019f);
    }

    // Update is called once per frame
    void Update()
    {
        Transform transform = gameObject.transform;
        GameObject line = new GameObject();
        Vector3 go_pos = transform.position;
        Vector3 forward = transform.forward;
        line.transform.position = go_pos;
        line.AddComponent<LineRenderer>();

        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.material.SetColor("_Color", Color.blue);
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.SetPosition(0, go_pos);
        lr.SetPosition(1, forward * 1000);
        GameObject.Destroy(line, delay);
        
        RaycastHit hit;
        if (Physics.Raycast(go_pos, forward, out hit, Mathf.Infinity))
        {
            localHit = hit.point;
            pointingAtDebug = hit.collider.gameObject;
            string hitObjectName = pointingAtDebug.name;
            
            if (hitObjectName == "Cat Lite" && OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
            {
                catLocationInd = (catLocationInd + 1) % catLocations.Length;
                cat.transform.position = catLocations[catLocationInd];
            }
            
            if ((pointingAtDebug.GetComponent<opencloseDoor>() != null || pointingAtDebug.GetComponent<opencloseDoor1>() != null || pointingAtDebug.GetComponent<opencloseStallDoor>() != null) && OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
            {
                pointingAtDebug.GetComponent<opencloseDoor>().changeState();
            }
        }
    }
}
