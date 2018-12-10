using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundResultUI : MonoBehaviour {
    private Text txt;
	
	void Awake () {
        txt.GetComponent<Text>();
        txt.text = "";
    }
	
	// Update is called once per frame
	void Update () {
        if (Bartok.S.phase != TurnPhase.gameOver)
        {
            txt.text = "";
            return;
        }//if
        Player cP = Bartok.CURRENT_PLAYER;
        if (cP == null || cP.type == PlayerType.human)
        {
            txt.text = "";
        }//if
        else
        {
            txt.text = "Player"+cP.playerNum+"won";
        }//else
    }
}
