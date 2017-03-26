using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrabAndDrop : MonoBehaviour {
    GameObject grabbedObject;
    public RawImage cursorGrab;
    public RawImage cursorDefault;
    public RawImage cursorGrabbed;
    public RawImage cursorBuy;
    Rigidbody rb;
    private UIScript uiscript;
    public AudioClip buyItem;
    private AudioSource source;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cursorDefault.enabled = true;
        cursorGrab.enabled = false;
        cursorGrabbed.enabled = false;
        cursorBuy.enabled = false;
        uiscript = GameObject.Find("UI").GetComponent<UIScript>();
        source = GetComponent<AudioSource>();
    }
    GameObject GetMouseHoverObject(float range)
    {
        Vector3 position = Camera.main.transform.position;
        RaycastHit raycastHit;
        Vector3 target = position + Camera.main.transform.forward * range;
        if (Physics.Linecast(position, target, out raycastHit))
        {
            if (!CanGrab(raycastHit.collider.gameObject))
                return null;
            return raycastHit.collider.gameObject;
        }
        else
            return null;
    }
    bool CanGrab(GameObject candidate)
    {
        return candidate.GetComponent<Rigidbody>() != null || candidate.tag.Contains("Shopitem");
    }
    void TryGrabObject(GameObject grabObject)
    {
        if (grabObject == null || !CanGrab(grabObject))
            return;
        if (transform.GetComponent<characterController>().items.ContainsKey(grabObject.tag))
        {
            if (transform.GetComponent<characterController>().money >= transform.GetComponent<characterController>().items[grabObject.tag])
            {
                if (grabObject.tag=="Shopitem ES")
                {
                    transform.GetComponent<characterController>().currentEScans += 1;
                    uiscript.updateESText(transform.GetComponent<characterController>().currentEScans);
                }
                else
                {
                    transform.GetComponent<characterController>().myVapes += 1;
                    uiscript.updateVapeText(transform.GetComponent<characterController>().myVapes);
                }
                transform.GetComponent<characterController>().money -= transform.GetComponent<characterController>().items[grabObject.tag];
                
                uiscript.updateMoneyText(transform.GetComponent<characterController>().money);
                source.PlayOneShot(buyItem, 1F);
                Destroy(grabObject);
            }

        }
        else if (!grabObject.tag.Contains("Shopitem"))
        {
          
            grabbedObject = grabObject;
            //grabbedObjectSize = grabObject.GetComponent<Renderer>().bounds.size.magnitude;
            grabbedObject.GetComponent<Collider>().enabled = false;
        }
    }
    void DropObject ()
    {
        if (grabbedObject == null)
            return;
        grabbedObject.GetComponent<Collider>().enabled = true;
        if (grabbedObject.GetComponent<Rigidbody>() != null)
        {
            grabbedObject.GetComponent<Rigidbody>().velocity =Camera.main.velocity+rb.velocity;
        }
        grabbedObject = null;
    }
    void Update () {

        if (transform.GetComponent<characterController>().enter == characterController.ENTER_BUYITEM)
            transform.GetComponent<characterController>().enter = characterController.ENTER_DEFAULT;
        if (grabbedObject != null)
        {
            cursorDefault.enabled = false;
            cursorGrab.enabled = false;
            cursorGrabbed.enabled = true;
            cursorBuy.enabled = false;

        }
        else
        {
            GameObject hoverObject = GetMouseHoverObject(1.5F);
            if (hoverObject == null)
            {
                cursorDefault.enabled = true;
                cursorGrab.enabled = false;
                cursorGrabbed.enabled = false;
                cursorBuy.enabled = false;
            }    
            else if (transform.GetComponent<characterController>().items.ContainsKey(hoverObject.tag))
            {
                cursorDefault.enabled = false;
                cursorGrab.enabled = false;
                cursorGrabbed.enabled = false;
                cursorBuy.enabled = true;
                if (hoverObject.tag.Contains("Shopitem") && transform.GetComponent<characterController>().enter != characterController.ENTER_BUYITEM)
                {
                    transform.GetComponent<characterController>().currentHoverName = hoverObject.tag;
                    transform.GetComponent<characterController>().enter = characterController.ENTER_BUYITEM;
                }
            }
            else
            {
                cursorDefault.enabled = false;
                cursorGrab.enabled = true;
                cursorGrabbed.enabled = false;
                cursorBuy.enabled = false;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (grabbedObject == null)
                TryGrabObject(GetMouseHoverObject(1.5F));
            else
                DropObject();
        }
        if (grabbedObject!=null)
        {
            Vector3 newPosition = gameObject.transform.position + Camera.main.transform.forward * 0.75f + new Vector3(0,0.2F,0);
            grabbedObject.transform.position = newPosition;
        }
	}
    void FixedUpdate() { 
}
}
