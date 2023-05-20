using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePart : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.ScrollRect scroll;
    private Player player;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        Vector3 cameraTargetPosition = Vector3.zero;
        //if (player != null)
        //{
        //    cameraTargetPosition = player.transform.position;
        //}
        //else if (samplePlayer != null)
        //{
        //    cameraTargetPosition = samplePlayer.transform.position;
        //}
        //camera.transform.position = cameraTargetPosition;
        //
        //cameraTargetPosition = (camera.transform as RectTransform).anchoredPosition3D;
        RectTransform rectTrans = this.transform as RectTransform;
        //cameraTargetPosition = new Vector3(Mathf.Clamp(cameraTargetPosition.x, cameraRange.rect.left + (Screen.width / 2), cameraRange.rect.right - (Screen.width / 2)), Mathf.Clamp(cameraTargetPosition.y, cameraRange.rect.yMin + (Screen.height / 2), cameraRange.rect.yMax - (Screen.height / 2)), 0);
        //(camera.transform as RectTransform).anchoredPosition3D = cameraTargetPosition + new Vector3(0, 0, -10);

        cameraTargetPosition = (player.transform as RectTransform).anchoredPosition;
        //cameraTargetPosition += new Vector3(cameraTargetPosition.x+(Screen.width/2),cameraTargetPosition.y+(Screen.height/2),0);
        //scroll.verticalNormalizedPosition = (cameraTargetPosition.y/cameraRange.rect.size.y);
        //scroll.horizontalNormalizedPosition = ((cameraTargetPosition.x-(Screen.width/2))/(cameraRange.rect.size.x-(Screen.width)));
        RectTransform scrollRectTrans = scroll.transform as RectTransform;
        scroll.content.anchoredPosition
            = new Vector3(
                Mathf.Clamp(-(cameraTargetPosition.x - (scrollRectTrans.rect.width / 2)), -(rectTrans.rect.xMax - (scrollRectTrans.rect.width)), -rectTrans.rect.xMin)
            , Mathf.Clamp(-(cameraTargetPosition.y - (scrollRectTrans.rect.height / 2)), -(rectTrans.rect.yMax - (scrollRectTrans.rect.height)), -rectTrans.rect.yMin)
            , 0);
        //if (!manualScroll)
        //{
        //    debugScrollValue = scroll.horizontalNormalizedPosition;
        //}
        //else
        //{
        //    scroll.horizontalNormalizedPosition = debugScrollValue;
        //}
    }
}
