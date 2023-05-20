using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Game : MonoBehaviour
{
    private const float gameBombLimit = 30.0f;
    private const float localBombLimit = 5.0f;
    private const float successCircleLimit = 0.5f;

    [SerializeField] private UnityEngine.UI.Text txTime;
    [SerializeField] private UnityEngine.UI.Text txAppear;
    [SerializeField] private UnityEngine.UI.Text txInput;
    [SerializeField] private GameObject goSuccessCircle;

    [SerializeField] private Keyboard keyboard;

    public int Score { get; private set; }

    private float timer;
    private float localBomb;
    private float successCircleBomb;

    private int level;
    private List<(int left,int right ,bool success)> history;

    private int curAppear;
    private int curAnswer;

    private int input;


    public System.Action actNextScene;


    public void StartScene()
    {
        this.gameObject.SetActive(true);

        timer = 0;
        localBomb = 0;
        successCircleBomb = 0;

        level = 1;

        history = new List<(int left, int right, bool success)>();

        curAppear = 0 -1;
        curAnswer = (-1 * level) - 1;

        input = -1;

        goSuccessCircle.SetActive(false);
    }

    public void EndScene()
    {
        goSuccessCircle.SetActive(false);

        Score = (int)(((float)history.Where(e=>e.success).Count() / (float)history.Count())*100);

        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        localBomb -= Time.deltaTime;
        successCircleBomb -= Time.deltaTime;

        if (localBomb<=0&&timer<gameBombLimit)
        {
            Lottery();
        }

        if(keyboard.GetKeyDown(KeyCode.Return))
        {
            Judge();
        }

        System.Enum.GetValues(typeof(KeyCode)).OfType<KeyCode>().Where(e=>KeyCode.Alpha0<=e&&e<=KeyCode.Alpha9).Where(e=>keyboard.GetKeyDown(e)).Select(e=>input=e-KeyCode.Alpha0).ToList();
        if(keyboard.GetKeyDown(KeyCode.Backspace))
        {
            input = -1;
        }
        txInput.text = 0<=input? input.ToString():string.Empty;
        
        if(successCircleBomb<=0)
        {
            if(goSuccessCircle.activeSelf)
            {
                goSuccessCircle.SetActive(false);
            }
        }

        if(gameBombLimit<=timer&&localBomb<=0&&!keyboard.GetKeyDown(KeyCode.Return))
        {
            EndScene();
            actNextScene();
        }

        System.TimeSpan ts = new System.TimeSpan(0, 0, (int)timer);
        txTime.text = ts.Minutes.ToString("D2")+" : "+ts.Seconds.ToString("D2");
    }

    private void Lottery()
    {
        ++curAppear;
        ++curAnswer;

        int answer = Random.Range(0,9);
        int left = Random.Range(0,answer);
        int right = answer - left;
        history.Add((left,right,false));
        localBomb = localBombLimit;
        txAppear.text = left.ToString()+"+"+right.ToString();
        input = -1;
    }

    private void Judge()
    {
        if(curAnswer<0)
        {
            return;
        }

        (int left, int right, bool success) page = history[curAnswer];
        if(page.left+page.right==input)
        {
            history[curAnswer] = (page.left,page.right,true);
            localBomb = 0.0f;
            successCircleBomb = successCircleLimit;
            goSuccessCircle.SetActive(true);
        }
    }
}
