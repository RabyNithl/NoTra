using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(UnityEngine.UI.Button))]
public class KeyboardButton : MonoBehaviour
{
    [SerializeField] private Keyboard keyboard;
    [SerializeField] private KeyCode key;

    private UnityEngine.UI.Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = this.GetComponent<UnityEngine.UI.Button>();
        button.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        button.onClick.AddListener(new UnityEngine.Events.UnityAction(OnClick));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick()
    {
        keyboard.OnClickButton(key);
    }
}
