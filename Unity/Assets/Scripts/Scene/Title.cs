using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField] private Keyboard keyboard;


    public System.Action actNextScene;


    public void StartScene()
    {
        this.gameObject.SetActive(true);
    }

    public void EndScene()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(keyboard.GetKeyDown(KeyCode.Return))
        {
            EndScene();
            actNextScene();
        }
    }
}
