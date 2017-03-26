using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Contact
{
    public string name;
    public bool buyingVape;
    public int nextBuyingVapeDay;
    public Contact(string n, int nextBuyingDay=0,bool buying=false)
    {
        name = n;
        buyingVape = buying;
        nextBuyingVapeDay = nextBuyingDay;
    }
}
public class characterController : MonoBehaviour {
    public float speed = 25.0F;
    private float walkspeed = 10.0F;
    public float force = 50.0F;
    public int currentES = 0;
    public int targetES;
    public const int esSatisfaction = 20;
    private const int esTargetLevelRise = 5;
    public int currentEScans = 0;
    public int myVapes = 0;
    public int fee=200;
    //public Vector3 direction = new Vector3(0.0f, 1.0f, 0.0f);
    private Rigidbody rb;
    float deltaTime = 0.0f;
    private float translation;
    private float straffe;
    public int enter = 0;
    public int money = 0;
    private float drinkingTick = 0f;
    private float drinkingDelay = 180f;
    public const int ENTER_DEFAULT=0,ENTER_SCHOOL= 4, ENTER_DOOR=1, ENTER_SLEEP=2,ENTER_BUYITEM=5,ENTER_CLOSEDOOR=3,ENTER_CURSORLOCKED=6,ENTER_CURSORUNLOCKED=7, ENTER_CONTACT=8;
    private int previousEnter = 0;
    private UIScript uiscript;
    public FadeManager fadeManager;
    private bool beenToSchoolToday = false;
    private const float earliestSchoolArrivalTime = 7.5f * 60f * 60f;
    private const float latestSchoolArrivalTime = 8.25f * 60f * 60f;
    private const float schoolDayEndingTime = 14.75f * 60f * 60f;
    private const int dailyMoneyFromParents = 420;

    public float fadeTime = 3f;
    public Dictionary<string, int> items = new Dictionary<string, int>();
    public AudioClip drinkES;
    public AudioClip buyItem;
    private AudioSource source;
    public Transform esPrefab;
    public Transform esShelfPrefab;
    public Transform vapeShelfPrefab;
    private Object VapeShelf;
    private Object ESshelf;
    private bool firstTry = true;
    public bool browsingPhone = false;
    public bool cursorLockedOnUI= false;
    private string currentContact;
    public string currentHoverName;
    private int lastDrinkDay = 0;
    public bool isAlive = true;
    public Dictionary<string, Contact> Contacts = new Dictionary<string, Contact>();

    //items.Add("Shopitem ES", 120);
    // Use this for initialization
    void Start() {
        isAlive = true;
        items["Shopitem ES"] = 120;
        items["Shopitem E-liquid"] = 400;
        enter = ENTER_DEFAULT;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = this.GetComponent<Rigidbody>();
       
        ConstructContacts();
        uiscript = GameObject.Find("UI").GetComponent<UIScript>();
        fadeManager = GameObject.Find("UI").GetComponent<FadeManager>();
        if (PlayerPrefs.GetInt("LoadingGame") == 2)
        {
            money = transform.GetComponent<SaveInformation>().LoadMoney();
            uiscript.days= transform.GetComponent<SaveInformation>().LoadDays()-1;
            currentEScans = transform.GetComponent<SaveInformation>().LoadEScans();
            myVapes = transform.GetComponent<SaveInformation>().LoadVape();
            targetES = uiscript.days * esTargetLevelRise + 50;
        }
        else
        {
            money = dailyMoneyFromParents+500;
            targetES = 50;

        }
        uiscript.updateMoneyText(money);
        uiscript.updateESText(currentEScans);
        uiscript.updateESslider(currentES, targetES);
        uiscript.updateVapeText(myVapes);
        uiscript.SetupContacts();
        source = GetComponent<AudioSource>();
        WakeUp();
        firstTry = false;
        
        //fadeManager.Fade(false, fadeTime);
    }
    private void ConstructContacts()
    {
        if (PlayerPrefs.HasKey("Contact 1_name") && PlayerPrefs.GetInt("LoadingGame") == 2)
        {
            int i = 1;
            while (i < 100)
            {
                if (!PlayerPrefs.HasKey("Contact " + i.ToString() + "_name"))
                    break;
                bool buyingVape = false;
                if (PlayerPrefs.GetInt("Contact " + i.ToString() + "_buyingvape") == 2)
                    buyingVape = true;
                Contacts["Contact " + i.ToString()] = new Contact(PlayerPrefs.GetString("Contact " + i.ToString() + "_name"), PlayerPrefs.GetInt("Contact " + i.ToString() + "__nextBuyingVapeDay"), buyingVape);
                i++;
            }
        }
        else
        {
            Contacts["Contact 1"] = new Contact("Vertti");
            Contacts["Contact 2"] = new Contact("Jonnemander");
            Contacts["Contact 3"] = new Contact("Joni");
            Contacts["Contact 4"] = new Contact("Make");
            Contacts["Contact 5"] = new Contact("Viljami");
            Contacts["Contact 6"] = new Contact("Pave");
            Contacts["Contact 7"] = new Contact("Huore");
        }

    }
    // Update is called once per frame

    public void exitExitPanel()
    {
        uiscript.exitPanel.SetActive(false);
        cursorLockedOnUI = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        enter = ENTER_DEFAULT;
    }
    void Update() {
        if (!isAlive)
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiscript.exitPanel.SetActive(!uiscript.exitPanel.activeSelf);
            cursorLockedOnUI = uiscript.exitPanel.activeSelf;
            if (uiscript.helpPanel.activeSelf)
            {
                uiscript.helpPanel.SetActive(false);
            }
            if (uiscript.exitPanel.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                enter = ENTER_CURSORUNLOCKED;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                enter = ENTER_DEFAULT;
            }
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            walkspeed = speed * 1.5F;
        }
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            walkspeed = speed;
        }
        if (Input.GetKey(KeyCode.Space) && isGrounded())
        {
            rb.velocity = new Vector3(0.0f, force, 0.0f);
        }
        if (Input.GetKeyDown(KeyCode.G) && (browsingPhone || uiscript.exitPanel.activeSelf || uiscript.helpPanel.activeSelf))
        {
            cursorLockedOnUI = !cursorLockedOnUI;
            if (!cursorLockedOnUI)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                enter = ENTER_CURSORLOCKED;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                enter = ENTER_CURSORUNLOCKED;
            }
            }

        if (Input.GetKeyDown(KeyCode.P))
            ClosePhone(!browsingPhone);
        if (Input.GetKey(KeyCode.F) && enter==ENTER_SCHOOL && uiscript.clockTime >= earliestSchoolArrivalTime && uiscript.clockTime <= latestSchoolArrivalTime && !beenToSchoolToday)
        {
            beenToSchoolToday = true;
            fadeManager.Fade(true, fadeTime);
            Invoke("ComeOutFromSchool", fadeTime);
        }
        if (Input.GetKey(KeyCode.R) && currentEScans>0 && (uiscript.clockTime > drinkingTick+drinkingDelay || uiscript.days > lastDrinkDay))
        {
            currentEScans -= 1;
            uiscript.updateESText(currentEScans);
            currentES += esSatisfaction;
            uiscript.updateESslider(currentES, targetES);
            drinkingTick = uiscript.clockTime;
            source.PlayOneShot(drinkES, 1F);
           
            Instantiate(esPrefab, new Vector3(transform.position.x, transform.position.y+0.25f, transform.position.z)+transform.forward*0.5f, Quaternion.identity);
            lastDrinkDay = uiscript.days;
        }
        if (Input.GetKey(KeyCode.F) && enter==ENTER_SLEEP && currentES>=targetES && !fadeManager.isInTransition)
        {
            fadeManager.Fade(true, fadeTime);
            Invoke("WakeUp", fadeTime);
        }
        if (Input.GetKey(KeyCode.F) && enter==ENTER_CONTACT && Contacts[currentContact].buyingVape && myVapes>0 && uiscript.days >= Contacts[currentContact].nextBuyingVapeDay)
        {
            Contacts[currentContact].buyingVape = false;
            Contacts[currentContact].nextBuyingVapeDay = uiscript.days + 4;
            myVapes -= 1;
            uiscript.updateVapeText(myVapes);
            money += items["Shopitem E-liquid"] + fee;
            uiscript.updateMoneyText(money);
            source.PlayOneShot(buyItem, 1F);

        }
        if (!fadeManager.isShowing)
        {
            translation = Input.GetAxis("Vertical");
            straffe = Input.GetAxis("Horizontal");
            float pythagoras = ((translation * translation) + (straffe * straffe));
            if (pythagoras > 1)
            {
                float magnitude = Mathf.Sqrt(pythagoras);
                float multiplier = 1 / magnitude;
                translation *= multiplier;
                straffe *= multiplier;
            }
            translation *= walkspeed;
            straffe *= walkspeed;
            transform.Translate(straffe * Time.deltaTime, 0f, translation * Time.deltaTime);
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }
    }
    public void ClosePhone(bool opening)
    {
        uiscript.phone.SetActive(opening);
        cursorLockedOnUI = opening;
        if (!opening)
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (enter == ENTER_CURSORUNLOCKED || enter == ENTER_CURSORLOCKED)
                enter = previousEnter;
            //enter = ENTER_CURSORLOCKED;
        }
        else
        {
            previousEnter = enter;
            Cursor.lockState = CursorLockMode.None;
            enter = ENTER_CURSORUNLOCKED;
        }
        Cursor.visible = opening;
        browsingPhone = !browsingPhone;
    }
    private void ComeOutFromSchool()
    {
        uiscript.clockTime = schoolDayEndingTime;
        fadeManager.Fade(false, fadeTime);
    }
    private void WakeUp()
    {
        uiscript.newDay();
        if (beenToSchoolToday)
            money += dailyMoneyFromParents;
        uiscript.updateMoneyText(money);
        beenToSchoolToday = false;
        currentES = 0;
        fadeManager.Fade(false, fadeTime);
        if (!firstTry)
        {
            Destroy((ESshelf as Transform).gameObject);
            Destroy((VapeShelf as Transform).gameObject);
            targetES += esTargetLevelRise;
        }
        uiscript.updateESslider(currentES, targetES);
        ESshelf = Instantiate(esShelfPrefab, new Vector3(-15.8f, 0.4f, 75.82f), Quaternion.identity);
        VapeShelf = Instantiate(vapeShelfPrefab, new Vector3(-13.671f, 0.5802f, 77.1935f), Quaternion.identity);
        transform.GetComponent<SaveInformation>().SaveData();
    }
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        if(enter!= ENTER_DEFAULT)
        {
            if (enter == ENTER_DOOR)
                text = "Press 'F' to open the door";
            else if (enter == ENTER_SLEEP && currentES >= targetES)
                text = "Press 'F' to sleep";
            else if (enter == ENTER_SLEEP)
                text = "Not enough ES to sleep";
            else if (enter == ENTER_SCHOOL)
            {
                if (uiscript.clockTime < earliestSchoolArrivalTime)
                    text = "School isn't open yet";
                else if (uiscript.clockTime >= earliestSchoolArrivalTime && uiscript.clockTime <= latestSchoolArrivalTime)
                    text = "Press 'F' to go to school. School starts at " + uiscript.getNiceTime(latestSchoolArrivalTime);
                else if (beenToSchoolToday)
                    text = "School day is over";
                else
                    text = "You're late! School started at " + uiscript.getNiceTime(latestSchoolArrivalTime) + ". Come tomorrow again.";
            }
            else if (enter == ENTER_BUYITEM)
            {
                string[] productName = currentHoverName.Split(' ');

                if (items[currentHoverName] <= money)
                    text = "Buy "+productName[1]+" for " + (items[currentHoverName] / 100.0F).ToString("F2") + " €";
                else
                    text = "No money to buy "+productName[1]+" for " + (items[currentHoverName] / 100.0F).ToString("F2") + " €";
            }
            else if (enter == ENTER_CLOSEDOOR)
                text = "Press 'F' to close the door";
            else if (enter == ENTER_CURSORLOCKED)
                text = "Press 'G' to focus on UI";
            else if (enter == ENTER_CURSORUNLOCKED)
                text = "Press 'G' to focus on game";
            else if (enter==ENTER_CONTACT)
            {
                if (Contacts[currentContact].nextBuyingVapeDay > uiscript.days)
                {
                    text = Contacts[currentContact].name + " hasn't consumed the previous liquid fully yet.";
                }
                else if (Contacts[currentContact].buyingVape)
                {
                    text = "Press 'F' to sell the vape liquid to " + Contacts[currentContact].name;
                }
                else
                {
                    text = Contacts[currentContact].name + " isn't buying vape liquid at the time";
                }
            }
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height/2+100, 200, 50), text);
        }
    }
    bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 0.45f);
    }
    void OnTriggerEnter(Collider other)
    {
        previousEnter = enter;
        if (other.gameObject.tag == "Bed")
        {
            
            enter = ENTER_SLEEP;
        }
        else if (other.gameObject.tag == "Door")
        {
            if (other.gameObject.GetComponent<doorOpening>().open)
                enter = ENTER_CLOSEDOOR;
            else
                enter = ENTER_DOOR;
        }
        else if (other.gameObject.tag == "School")
            enter = ENTER_SCHOOL;
        else if (other.gameObject.tag.Contains("Contact"))
        {
            currentContact = other.gameObject.tag;
            enter = ENTER_CONTACT;
        }
    }
    void OnTriggerExit(Collider other)
    {
        enter = ENTER_DEFAULT;
        previousEnter = ENTER_DEFAULT;
    }
}
