using UnityEngine;
using System.Collections;

public class doorOpening : MonoBehaviour {
    public float smooth = 2.0F;
    public float DoorOpenAngle = 90.0F;
    public bool open;
    private bool enter;

    private Vector3 defaultRot;
    private Vector3 openRot;
    


    //Activate the Main function when player is near the door
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enter = true;
        }
    }

    //Deactivate the Main function when player is go away from door
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enter = false;
        }
    }
    // Use this for initialization
    void Start () {
        defaultRot = transform.eulerAngles;
        openRot = new Vector3(defaultRot.x, defaultRot.y + DoorOpenAngle, defaultRot.z);
    }
	
	// Update is called once per frame
	void Update () {
        if (open)
        {
            //Open door
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
        }
        else
        {
            //Close door
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaultRot, Time.deltaTime * smooth);
        }

        if (Input.GetKeyDown("f") && enter)
        {
            open = !open;
        }
    }
}
