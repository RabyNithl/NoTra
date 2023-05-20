using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Keyboard : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Toggle toggle;
    [SerializeField] private GameObject goBody;

    private Dictionary<KeyCode,List<KeyCode>> dictKeyBinder;
    private Dictionary<KeyCode,(bool cur, bool pre)> dictInput;

    private bool isDetectInputScreen = false;


    public bool GetKeyDown(KeyCode key)
    {
        return 0<dictInput.Where(e=>e.Key==ToKey(key)).Where(e=>e.Value.cur&&!e.Value.pre).Count();
    }

    void Start()
    {
        dictKeyBinder = new Dictionary<KeyCode, List<KeyCode>>()
            {
                {KeyCode.Return     ,   new List<KeyCode>(){KeyCode.Return      ,KeyCode.KeypadEnter    }}  ,
                {KeyCode.Backspace  ,   new List<KeyCode>(){KeyCode.Backspace   ,                       }}  ,
                {KeyCode.Alpha0     ,   new List<KeyCode>(){KeyCode.Alpha0      ,KeyCode.Keypad0        }}  ,
                {KeyCode.Alpha1     ,   new List<KeyCode>(){KeyCode.Alpha1      ,KeyCode.Keypad1        }}  ,
                {KeyCode.Alpha2     ,   new List<KeyCode>(){KeyCode.Alpha2      ,KeyCode.Keypad2        }}  ,
                {KeyCode.Alpha3     ,   new List<KeyCode>(){KeyCode.Alpha3      ,KeyCode.Keypad3        }}  ,
                {KeyCode.Alpha4     ,   new List<KeyCode>(){KeyCode.Alpha4      ,KeyCode.Keypad4        }}  ,
                {KeyCode.Alpha5     ,   new List<KeyCode>(){KeyCode.Alpha5      ,KeyCode.Keypad5        }}  ,
                {KeyCode.Alpha6     ,   new List<KeyCode>(){KeyCode.Alpha6      ,KeyCode.Keypad6        }}  ,
                {KeyCode.Alpha7     ,   new List<KeyCode>(){KeyCode.Alpha7      ,KeyCode.Keypad7        }}  ,
                {KeyCode.Alpha8     ,   new List<KeyCode>(){KeyCode.Alpha8      ,KeyCode.Keypad8        }}  ,
                {KeyCode.Alpha9     ,   new List<KeyCode>(){KeyCode.Alpha9      ,KeyCode.Keypad9        }}  ,
            };

        dictInput = new Dictionary<KeyCode,(bool cur,bool pre)>()
            {
                {KeyCode.Return     ,(false,false)}  ,
                {KeyCode.Backspace  ,(false,false)}  ,
                {KeyCode.Alpha0     ,(false,false)}  ,
                {KeyCode.Alpha1     ,(false,false)}  ,
                {KeyCode.Alpha2     ,(false,false)}  ,
                {KeyCode.Alpha3     ,(false,false)}  ,
                {KeyCode.Alpha4     ,(false,false)}  ,
                {KeyCode.Alpha5     ,(false,false)}  ,
                {KeyCode.Alpha6     ,(false,false)}  ,
                {KeyCode.Alpha7     ,(false,false)}  ,
                {KeyCode.Alpha8     ,(false,false)}  ,
                {KeyCode.Alpha9     ,(false,false)}  ,
            };

        toggle.onValueChanged = new UnityEngine.UI.Toggle.ToggleEvent();
        toggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(b=>goBody.SetActive(b)));
        toggle.isOn = !toggle.isOn;
        toggle.isOn = !toggle.isOn;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDetectInputScreen)
        {
            dictInput = dictInput.Select(
                e=>
                {
                    return new KeyValuePair<KeyCode,(bool cur,bool pre)>(
                        e.Key,(
                            0<ToKeys(e.Key).Where(f=>Input.GetKey(f)).Count()
                            ,e.Value.cur
                            )); 
                }).ToDictionary(e=>e.Key,e=>e.Value);
        }
        isDetectInputScreen = false;
    }

    private KeyCode ToKey(KeyCode key)
    {
        return dictKeyBinder.Where(e=>0<e.Value.Count(f=>f==key)).FirstOrDefault().Key;
    }

    private List<KeyCode> ToKeys(KeyCode key)
    {
        return dictKeyBinder[key];
    }

    public void OnClickButton(KeyCode key)
    {
        dictInput[ToKey(key)] = (true,false);

        isDetectInputScreen = true;
    }
}
