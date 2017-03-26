using UnityEngine;
using System.Collections;

public class camMouseLook : MonoBehaviour {
    Vector2 mouseLook;
    Vector2 smoothV;
    public float sensitivity = 5.0f;
    public float smoothing = 2.0f;
    GameObject character;

	// Use this for initialization
	void Start () {
        character = this.transform.parent.gameObject;
	}
	// Update is called once per frame
	void Update () {
        float x = 0, y = 0;
        if (!character.GetComponent<characterController>().cursorLockedOnUI && character.GetComponent<characterController>().isAlive)
        {
            x = Input.GetAxisRaw("Mouse X");
            y = Input.GetAxisRaw("Mouse Y");
        }
        var md = new Vector2(x,y);
        //else
        //    var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        mouseLook += smoothV;
        if (mouseLook.y > 90)
            mouseLook.Set(mouseLook.x, 90);
        else if (mouseLook.y < -90)
            mouseLook.Set(mouseLook.x, -90);
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
	}
}
