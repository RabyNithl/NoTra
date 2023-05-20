using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private Title title;
    [SerializeField] private Game game;
    [SerializeField] private Result result;


    // Start is called before the first frame update
    void Start()
    {
        title.gameObject.SetActive(true);
        game.gameObject.SetActive(false);
        result.gameObject.SetActive(false);

        title.actNextScene = game.StartScene;
        game.actNextScene = ()=> {result.score=game.Score;result.StartScene();};
        result.actNextScene = game.StartScene;
    }
}
