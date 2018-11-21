using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour {

	public static Scoreboard S;

    [Header("Set in Inspector")]
    public GameObject prefabFloatingScore;

    [Header("Set Dynamically")]
    [SerializeField] private int _score = 0;
    [SerializeField] private string _scoreString;

    private Transform canvasTrans;

    public int score    {
        get        {
            return (_score);
        }//get
        set        {
            _score = value;
            scoreString = _score.ToString();//book says you "NO" 
        }//set
    }//public int

    public string scoreString    {
        get        {
            return (_scoreString);
        }//get
        set
        {
            _scoreString = value;
            GetComponent<Text>().text = _scoreString;
        }//set
    }//public string

    void Awake()    {
        if (S == null)        {
            S = this;
        }//if
        else        {
            Debug.LogError("ERROR: Scoreboard.Awake(): S is already...");
        }//else

        canvasTrans = transform.parent;
    }//awake

    public void FSCallback(FloatingScore fs)    {
        score += fs.score;
    }//public void

    public FloatingScore CreateFloatingScore(int amt ,List<Vector2> pts)    {
        GameObject go = Instantiate<GameObject>(prefabFloatingScore);
        go.transform.SetParent(canvasTrans);
        FloatingScore fs = go.GetComponent<FloatingScore>();
        fs.score = amt;
        fs.reportFinishTo = this.gameObject;
        fs.Init(pts);
        return (fs);
    }//public floatingScore



}//class
