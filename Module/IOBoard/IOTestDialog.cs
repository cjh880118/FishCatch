using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IOTestDialog : MonoBehaviour
{
    public Button Power_On = null;
    public Button Power_Off = null;

    public Button Damage_On = null;
    public Button Damage_Off = null;
    public InputField Damage_Input = null;

    public Button Led_On = null;
    public Button Led_Off = null;
    public InputField Led_Input = null;

    public Text StateText = null;


    // Use this for initialization
    void Start ()
    {
        Power_On.onClick.AddListener(OnPowerOnClick);
        Power_Off.onClick.AddListener(OnPowerOffClick);

        Damage_On.onClick.AddListener(OnDamageOnClick);
        Damage_Off.onClick.AddListener(OnDamageOffClick);

        Led_On.onClick.AddListener(OnLedOnClick);
        Led_Off.onClick.AddListener(OnLedOffClick);

        StateText.text = "";

       Message.AddListener<IOStateMsg>(OnIOState);
    }

    private void Update()
    {
        if( Input.GetKeyDown(KeyCode.F1))
        {
            this.transform.Find("View").Find("Input").gameObject.SetActive(!this.transform.Find("View").Find("Input").gameObject);
        }
    }

    void OnIOState(IOStateMsg msg)
    {
        StateText.text = msg.Desc +"\n"+ StateText.text;
    }

    void OnPowerOnClick()
    {
        Message.Send<IO_PowerMsg>(new IO_PowerMsg(true));
    }

    void OnPowerOffClick()
    {
        Message.Send<IO_PowerMsg>(new IO_PowerMsg(false));
    }

    void OnDamageOnClick()
    {
        int index = int.Parse(Damage_Input.text);
        Message.Send<IO_DamageMsg>(new IO_DamageMsg(index, true));
    }

    void OnDamageOffClick()
    {
        int index = int.Parse(Damage_Input.text);
        Message.Send<IO_DamageMsg>(new IO_DamageMsg(index, false));
    }

    void OnLedOnClick()
    {
        int index = int.Parse(Led_Input.text);
        Message.Send<IO_LedMsg>(new IO_LedMsg(index, true));
    }

    void OnLedOffClick()
    {
        int index = int.Parse(Led_Input.text);
        Message.Send<IO_LedMsg>(new IO_LedMsg(index, false));
    }
}
