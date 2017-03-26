using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour {
    public Text clockText;
    public float clockTime;
    public Text moneyText;
    public Text esText;
    public Text dayText;
    public Text vapeText;
    public Slider esSlider;
    public int days;
    public GameObject phone;
    private string currentScreen = "StartScreen";
    public GameObject contactButton;
    public GameObject Player;
    public GameObject exitPanel;
    public GameObject helpPanel;
    public GameObject deathPanel;
    public Text deathScoreText;
    void Start()
    {
        days = 0;
        clockTime = 7.5F * 60F * 60F;
        updateDayText();
        phone.SetActive(false);
        //SetupContacts();
        
    }
    public void SetupContacts()
    {
        int itemCount = Player.GetComponent<characterController>().Contacts.Count;

        RectTransform rowRectTransform = contactButton.GetComponent<RectTransform>();
        RectTransform containerRectTransform = transform.Find("Phone/ContactsScreen/ContactPanel/Scrollable").GetComponent<RectTransform>();
        float width = containerRectTransform.rect.width;
        float ratio = width / rowRectTransform.rect.width;
        float height = rowRectTransform.rect.height * ratio;
        int rowCount = itemCount;
        if (itemCount % rowCount > 0)
            rowCount++;
        float scrollHeight = height * rowCount;
        containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, Mathf.Min((containerRectTransform.rect.height-scrollHeight),0));
        containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, 0);
        int i = 0;
        foreach(string name in Player.GetComponent<characterController>().Contacts.Keys)
        {
            GameObject newContact = Instantiate(contactButton) as GameObject;
            newContact.GetComponentInChildren<Text>().text = Player.GetComponent<characterController>().Contacts[name].name;
            newContact.transform.SetParent(transform.Find("Phone/ContactsScreen/ContactPanel/Scrollable").transform);
            Button b = newContact.GetComponent<Button>();
            string captured = name;

            b.onClick.AddListener(() => OpenContact(captured));
            RectTransform rectTransform = newContact.GetComponent<RectTransform>();
            float x = -containerRectTransform.rect.width / 2 ;
            float y = containerRectTransform.rect.height / 2 - height * (i+1);
            rectTransform.offsetMin = new Vector2(x, y);
            x = rectTransform.offsetMin.x + width;
            y = rectTransform.offsetMin.y + height;
            rectTransform.offsetMax = new Vector2(x, y);
            i++;
        }
    }
    public void SellVape(string name)
    {
        //if (!Player.GetComponent<characterController>().Contacts[name].buyingVape)
        //{
        Player.GetComponent<characterController>().Contacts[name].buyingVape = true;
        updateContactStatusText(name);
        //}
    }
    void OpenContact(string name)
    {
        GoToScreen("ContactScreen");
        transform.Find("Phone/ContactScreen/NameText").GetComponent<Text>().text = Player.GetComponent<characterController>().Contacts[name].name;
        transform.Find("Phone/ContactScreen/VapeButton").GetComponent<Button>().onClick.RemoveAllListeners();
        transform.Find("Phone/ContactScreen/VapeButton").GetComponent<Button>().onClick.AddListener(() => SellVape(name));
        updateContactStatusText(name);
    }
    void updateContactStatusText(string name)
    {
        if (Player.GetComponent<characterController>().Contacts[name].nextBuyingVapeDay > days)
            transform.Find("Phone/ContactScreen/StatusText").GetComponent<Text>().text = "Status: has already";
        else if (Player.GetComponent<characterController>().Contacts[name].buyingVape)
            transform.Find("Phone/ContactScreen/StatusText").GetComponent<Text>().text = "Status: waiting for me";
        else
            transform.Find("Phone/ContactScreen/StatusText").GetComponent<Text>().text = "Status: no deal yet";
    }
    void OnGUI()
    {
        clockText.text = getNiceTime(clockTime);
    }
    public string getNiceTime(float givenTime)
    {
        int hours = Mathf.FloorToInt(givenTime / (60F * 60F));
        int minutes = Mathf.FloorToInt((givenTime - hours * 60F * 60F) / 60F);
        return string.Format("{0:00}:{1:00}", hours, minutes);
    }
    void Update()
    {
        clockTime += Time.deltaTime * 40;
        if (clockTime >= 24f*60*60 && Player.GetComponent<characterController>().isAlive)
        {
            Die();
        }
    }
    void FixedUpdate()
    {
        

    }
    public void updateMoneyText(int money)
    {
        moneyText.text = "Massit: " + (money / 100.0F).ToString("F2");
    }
    public void updateVapeText(int vapes)
    {
        vapeText.text = "Vapes: " + vapes.ToString();
    }
    public void updateESText(int ES)
    {
        esText.text = "ES: " + ES.ToString();
    }
    public void updateDayText()
    {
        dayText.text = "Day: " + days.ToString();
    }
    public void updateESslider(int currentES,int targetES)
    {
        esSlider.maxValue = targetES;
        esSlider.value = currentES;

    }
    public void ExitGame()
    {
        //Application.Quit();
        SceneManager.LoadScene(0);
    }
    public void ExitHelp()
    {
        helpPanel.SetActive(false);
        exitPanel.SetActive(true);
    }
    public void ExitExitmenu()
    {
        //exitPanel.SetActive(false);
        Player.GetComponent<characterController>().exitExitPanel();
    }
    public void OpenHelp()
    {
        helpPanel.SetActive(true);
        exitPanel.SetActive(false);
    }
    public void newDay()
    {
        clockTime = 7.5F * 60F * 60F;
        days += 1;
        updateDayText();
    }
    public void GoToScreen(string targetScreen)
    {
        transform.Find("Phone/" + currentScreen).gameObject.SetActive(false);
        transform.Find("Phone/" + targetScreen).gameObject.SetActive(true);
        currentScreen = targetScreen;
    }
    public void Die()
    {
        Player.GetComponent<characterController>().isAlive = false;
        if (Player.GetComponent<characterController>().browsingPhone)
            Player.GetComponent<characterController>().ClosePhone(false);
        transform.GetComponent<FadeManager>().Fade(true, 3f);
        deathPanel.SetActive(true);
        deathScoreText.text = "Days survived: " + days.ToString();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void StartAgain()
    {
        PlayerPrefs.SetInt("LoadingGame", 1);
        deathPanel.SetActive(false);
        SceneManager.LoadScene(1);
    }
    public void LoadLastNight()
    {
        deathPanel.SetActive(false);
        PlayerPrefs.SetInt("LoadingGame", 2);
        SceneManager.LoadScene(1);
    }
}
