using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Stage : MonoBehaviour
{
    public enum State
    {
        Start,
        Restart,
        Play,
    }

    public enum Result
    {
        Yet,
        Clear,
        Dead,
    }

    [System.Serializable]
    public class ObjectDatas
    {
        public List<ObjectData> objectDatas;
    }

    [System.Serializable]
    public class ObjectData
    {
        public enum EType
        {
            Invalid = -1,

            StartPosition   =   50  ,
            Home            =   60  ,
            Ground          =   100 ,
            DeathArea       =   200 ,
        }

        [System.Serializable]
        public struct Vector2
        {
            public static Vector2 ToVector2(UnityEngine.Vector2 v)
            {
                return new Vector2(){x=v.x,y=v.y};
            }
            public static UnityEngine.Vector2 ToVector2(Vector2 v)
            {
                return new UnityEngine.Vector2(){x=v.x,y=v.y};
            }

            public float x;
            public float y;
        }

        public EType type;
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
    }

    [System.Serializable]
    public class DebugObjectData
    {
        public ObjectData.EType type;
        public GameObject go;
    }


    [SerializeField] private Player samplePlayer;
    [SerializeField] private OneShotSound sampleDeathSE;
    [SerializeField] private OneShotSound sampleClearSE;
    [SerializeField] private GameObject sampleGround;
    [SerializeField] private Home sampleHome;
    [SerializeField] private DeathArea sampleDeathArea;
    [SerializeField] private RectTransform sampleStartPosition;


    [SerializeField] private Canvas world;
    [SerializeField] private UnityEngine.UI.ScrollRect scroll;
    [SerializeField] private Camera camera;
    [SerializeField] private AudioSource bgm;
    [SerializeField] private Home home;
    [SerializeField] private List<DeathArea> deathAreas;


    [SerializeField] private RectTransform startPosition;
    [SerializeField, System.Obsolete()] private RectTransform cameraRange;


    public ObjectDatas objectDatas = null;
    public int iStageData = 1;

    public Result _Result { get; private set; }
    public int Life { get; private set; }


    private State state = State.Start;

    private Player player;
    private List<GameObject> createdObjectData = null;

    private Home managedHome;
    private List<DeathArea> managedDeathAreas;
    private RectTransform managedStartPosition;


    public System.Action onStageEnded;
    public System.Action onLifeChanged;


    public bool manualScroll = false;
    public float debugScrollValue = 0;


    public List<GameObject> debugObjectDatas;
    public List<DebugObjectData> debugObjectDatasEx;


    // Start is called before the first frame update
    public void ManualStart()
    {
        this.gameObject.SetActive(true);

        //world.renderMode = RenderMode.ScreenSpaceCamera;
        //world.worldCamera = camera;
        //world.worldCamera = null;
        //world.renderMode = RenderMode.WorldSpace;

        scroll.content = this.transform as RectTransform;

        state = State.Start;

        managedStartPosition    =   null;
        managedHome             =   null;
        managedDeathAreas       =   null;        

        if(objectDatas!=null&&objectDatas.objectDatas!=null)
        {
            createdObjectData = objectDatas.objectDatas.Where(e=>e!=null).Select(
                e=>
                {
                    GameObject go = null;
                    switch(e.type)
                    {
                        case ObjectData.EType.Ground:
                            go = Instantiate(sampleGround,this.transform);
                            break;
                        case ObjectData.EType.StartPosition:
                            RectTransform rtSP = Instantiate(sampleStartPosition,this.transform);
                            go = rtSP.gameObject;
                            managedStartPosition = rtSP;
                            break;                            
                        case ObjectData.EType.Home:
                            Home h = Instantiate(sampleHome,this.transform);
                            go = h.gameObject;
                            managedHome = h;
                            break;
                        case ObjectData.EType.DeathArea:
                            DeathArea da = Instantiate(sampleDeathArea,this.transform);
                            go = da.gameObject;
                            if(managedDeathAreas==null)
                            {
                                managedDeathAreas = new List<DeathArea>();
                            }
                            managedDeathAreas.Add(da);
                            break;
                    }
                    if(go!=null)
                    {
                        go.SetActive(true);
                        RectTransform rt = go.transform as RectTransform;
                        rt.anchoredPosition = ObjectData.Vector2.ToVector2(e.anchoredPosition);
                        rt.sizeDelta = ObjectData.Vector2.ToVector2(e.sizeDelta);
                        BoxCollider2D bc2 = go.GetComponent<BoxCollider2D>();
                        if(bc2!=null)
                        {
                            bc2.size = rt.sizeDelta;
                        }
                        return go;
                    }
                    return go;
                }
                ).ToList();
        }

        managedHome             =   managedHome             !=  null?   managedHome             :   home            ;
        managedDeathAreas       =   managedDeathAreas       !=  null?   managedDeathAreas       :   deathAreas      ;
        managedStartPosition    =   managedStartPosition    !=  null?   managedStartPosition    :   startPosition   ;

        managedHome.gameObject.SetActive(true);
        managedDeathAreas.Select(e=>{e.gameObject.SetActive(true);return e;}).ToList();
        managedStartPosition.gameObject.SetActive(true);

        managedHome.actOnCollisionPlayerEnter =
            () =>
            {
                Instantiate(sampleClearSE,this.transform).gameObject.SetActive(true);
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                player.enabled = false;

                TimerAction.TimerSet(2,()=>{Destroy(player.gameObject);EndStage(Result.Clear);});
            };
        managedDeathAreas.ForEach(e=>e.actOnCollisionPlayerEnter = 
            () =>
            {
                if(!player.IsNoCollision)
                {
                    return;
                }
                Instantiate(sampleDeathSE,this.transform).gameObject.SetActive(true);
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation|RigidbodyConstraints2D.FreezePositionX;
                player.enabled = false;

                if(0<Life)
                {
                    TimerAction.TimerSet(1,()=>{Destroy(player.gameObject);state=State.Restart;});
                }
                else
                {
                    TimerAction.TimerSet(2,()=>{Destroy(player.gameObject);EndStage(Result.Dead);});
                }
            }
        );
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(2))
        {
            string s = JsonUtility.ToJson(new ObjectDatas(){objectDatas=debugObjectDatas.Where(e=>e!=null).Select(e=>e.transform as RectTransform).Where(e=>e!=null).Select(e=>new ObjectData(){type=ObjectData.EType.Ground,anchoredPosition=ObjectData.Vector2.ToVector2(e.anchoredPosition),sizeDelta=ObjectData.Vector2.ToVector2(e.sizeDelta)}).ToList()});
            s = JsonUtility.ToJson(new ObjectDatas(){objectDatas=debugObjectDatasEx.Where(e=>e!=null&&e.go!=null).Select(e=>(e.type,e.go.transform as RectTransform)).Where(e=>e.Item2!=null).Select(e=>new ObjectData(){type=e.type,anchoredPosition=ObjectData.Vector2.ToVector2(e.Item2.anchoredPosition),sizeDelta=ObjectData.Vector2.ToVector2(e.Item2.sizeDelta)}).ToList()});

            Debug.LogWarning(s);
        }

        switch(state)
        {
            case State.Start :
                bgm.Stop();
                bgm.loop = true;
                bgm.time = 0;
                bgm.Play();
                player = Instantiate(samplePlayer,this.transform);
                player.gameObject.SetActive(true);
                (player.transform as RectTransform).anchoredPosition = managedStartPosition.anchoredPosition;
                SetLife(3);
                state = State.Play;
                _Result = Result.Yet;
                break;
            case State.Restart :
                bgm.Stop();
                bgm.loop = true;
                bgm.time = 0;
                bgm.Play();
                player = Instantiate(samplePlayer,this.transform);
                player.gameObject.SetActive(true);
                (player.transform as RectTransform).anchoredPosition = managedStartPosition.anchoredPosition;
                SetLife(Life-1);
                state = State.Play;
                break;
            case State.Play:
                if(false)
                {

                }
                break;
        }

        Vector3 cameraTargetPosition = Vector3.zero;
        if(player!=null)
        {
            cameraTargetPosition = player.transform.position;
        }
        else if(samplePlayer!=null)
        {
            cameraTargetPosition = samplePlayer.transform.position;
        }
        camera.transform.position = cameraTargetPosition;

        cameraTargetPosition = (camera.transform as RectTransform).anchoredPosition3D;
        RectTransform rectTrans = this.transform as RectTransform;
        cameraTargetPosition = new Vector3(Mathf.Clamp(cameraTargetPosition.x,cameraRange.rect.left+(Screen.width/2),cameraRange.rect.right-(Screen.width/2)),Mathf.Clamp(cameraTargetPosition.y,cameraRange.rect.yMin+(Screen.height/2),cameraRange.rect.yMax-(Screen.height/2)),0);
        (camera.transform as RectTransform).anchoredPosition3D = cameraTargetPosition + new Vector3(0,0,-10);

        cameraTargetPosition = (player.transform as RectTransform).anchoredPosition;
        //cameraTargetPosition += new Vector3(cameraTargetPosition.x+(Screen.width/2),cameraTargetPosition.y+(Screen.height/2),0);
        //scroll.verticalNormalizedPosition = (cameraTargetPosition.y/cameraRange.rect.size.y);
        //scroll.horizontalNormalizedPosition = ((cameraTargetPosition.x-(Screen.width/2))/(cameraRange.rect.size.x-(Screen.width)));
        RectTransform scrollRectTrans = scroll.transform as RectTransform;
        scroll.content.anchoredPosition
            = new Vector3(
                Mathf.Clamp(-(cameraTargetPosition.x-(scrollRectTrans.rect.width    /2))    ,-(rectTrans.rect.xMax-(scrollRectTrans.rect.width ))   ,-rectTrans.rect.xMin   )
            ,   Mathf.Clamp(-(cameraTargetPosition.y-(scrollRectTrans.rect.height   /2))    ,-(rectTrans.rect.yMax-(scrollRectTrans.rect.height))   ,-rectTrans.rect.yMin   )
            ,   0);
        if(!manualScroll)
        {
            debugScrollValue = scroll.horizontalNormalizedPosition;
        }
        else
        {
            scroll.horizontalNormalizedPosition = debugScrollValue;
        }
    }

    public void EndStage(Result result)
    {
        if(_Result!=Result.Yet)
        {
            return;
        }

        _Result = result;
        bgm.Stop();

        createdObjectData?.Select(e=>{Destroy(e);return e;}).ToList();
        this.gameObject.SetActive(false);
        onStageEnded?.Invoke();
    }

    private void SetLife(int life)
    {
        this.Life = life;
        onLifeChanged?.Invoke();
    }
}
