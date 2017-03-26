using UnityEngine;
using System.Collections;

public class SaveInformation : MonoBehaviour {
    public void SaveData()
    {
        characterController charController = transform.GetComponent<characterController>();
        PlayerPrefs.SetInt("Days", GameObject.Find("UI").GetComponent<UIScript>().days);
        PlayerPrefs.SetInt("Money", charController.money);
        PlayerPrefs.SetInt("Vape", charController.myVapes);
        PlayerPrefs.SetInt("EScans", charController.currentEScans);
        foreach (string name in charController.Contacts.Keys)
        {
            PlayerPrefs.SetString(name + "_name", charController.Contacts[name].name);
            if (charController.Contacts[name].buyingVape)
                PlayerPrefs.SetInt(name + "_buyingvape", 2);
            else
                PlayerPrefs.SetInt(name + "_buyingvape", 1);
            PlayerPrefs.SetInt(name + "_nextBuyingVapeDay", charController.Contacts[name].nextBuyingVapeDay);
        }

    }
    public int LoadMoney()
    {
        return PlayerPrefs.GetInt("Money");
    }
    public int LoadDays()
    {
        return PlayerPrefs.GetInt("Days");
    }
    public int LoadVape()
    {
        return PlayerPrefs.GetInt("Vape");
    }
    public int LoadEScans()
    {
        return PlayerPrefs.GetInt("EScans");
    }
    
}
