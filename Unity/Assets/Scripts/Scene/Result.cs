using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Text txScore;

    [SerializeField] private Keyboard keyboard;


    public int score;
    public System.Action actNextScene;


    public void StartScene()
    {
        this.gameObject.SetActive(true);

        txScore.text = "正解率 : "+score.ToString()+"%\nEnter To Retry.";
    }

    public void EndScene()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (keyboard.GetKeyDown(KeyCode.Return))
        {
            EndScene();
            actNextScene();
        }
    }
}
