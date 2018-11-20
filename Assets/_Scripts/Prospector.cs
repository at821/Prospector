using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Prospector : MonoBehaviour {

	static public Prospector 	S;

	[Header("Set in Inspector")]
	public TextAsset			deckXML;
    public TextAsset layoutXML;
    public float xOffset = 3;
    public float yOffset = -2.5f;
    public Vector3 layoutCenter;

	[Header("Set Dynamically")]
	public Deck					deck;
    public Layout layout;
    public List<CardProspector> drawPile;
    public Transform layoutAnchor;
    public CardProspector target;
    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;



	void Awake(){
		S = this;
	}

	void Start() {
		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);
        Deck.Shuffle(ref deck.cards);

        /*   Card c;
            for (int cNum=0; cNum<deck.cards.Count; cNum++)
            {
                c = deck.cards[cNum];
                c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
            }
        */

        layout = GetComponent<Layout>();
        layout.ReadLayout(layoutXML.text);

        drawPile = ConvertListCardsToListCardProspectors(deck.cards);
        LayoutGame();
	}

    List<CardProspector>ConvertListCardsToListCardProspectors(List<Card> lCD)    {
        List<CardProspector>lCP = new List<CardProspector>();
        CardProspector tCP;
        foreach(Card tCD in lCD)        {
            tCP = tCD as CardProspector;
            lCP.Add(tCP);
        }//foreach
    return(lCP);
    }

    CardProspector Draw()    {
        CardProspector cd = drawPile[0];
        drawPile.RemoveAt(0);
        return (cd);
    }//card

    void LayoutGame()    {
        if (layoutAnchor ==null)        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }//if

        CardProspector cp;

        foreach (SlotDef tSD in layout.slotDefs)        {
            cp = Draw();
            cp.faceUp = tSD.faceUp;
            cp.transform.parent = layoutAnchor;

            cp.transform.localPosition = new Vector3(
                layout.multiplier.x * tSD.x,
                layout.multiplier.y * tSD.y,
                -tSD.layerID);

            cp.layoutID = tSD.id;
            cp.slotDef = tSD;

            cp.state = eCardState.tableau;

            cp.SetSortingLayerName(tSD.layerName);

            tableau.Add(cp);
        }//foreach

        foreach (CardProspector tCP in tableau)            {
            foreach (int hid in tCP.slotDef.hiddenBy)                {
                 cp = FindCardByLayoutID(hid);
                 tCP.hiddenBy.Add(cp);
            }//foreach
        }//foreach


            MoveToTarget(Draw());
            UpdateDrawPile();
     }  //layout

    CardProspector FindCardByLayoutID(int layoutID)
    {
        foreach (CardProspector tCP in tableau)
        {
            if (tCP.layoutID == layoutID)
            {
                return (tCP);
            }//if

        }//foreach
        return (null);
    }//card

    void SetTableauFaces()
    {
        foreach (CardProspector cd in tableau)
        {
            bool faceUp = true;

            foreach (CardProspector cover in cd.hiddenBy)
            {
                if (cover.state == eCardState.tableau)
                {
                    faceUp = false;
                }//if
            }//foreach
            cd.faceUp = faceUp;
        }//foreach
    }//void
        
  
    
    void MoveToDiscard(CardProspector cd)    {
        cd.state = eCardState.discard;
        discardPile.Add(cd);
        cd.transform.parent = layoutAnchor;

        cd.transform.localPosition = new Vector3(
            layout.multiplier.x + layout.discardPile.x,
            layout.multiplier.y + layout.discardPile.y,
            -layout.discardPile.layerID + .5f);
        cd.faceUp = true;

        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(-100 + discardPile.Count);
    }//void

    void MoveToTarget(CardProspector cd)    {
        if (target != null) MoveToDiscard(target);

        target = cd;

        cd.state = eCardState.target;
        cd.transform.parent = layoutAnchor;

        cd.transform.localPosition = new Vector3 (
            layout.multiplier.x + layout.discardPile.x,
            layout.multiplier.y + layout.discardPile.y,
            -layout.discardPile.layerID);

        cd.faceUp = true;

        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(0);
    }//void

    void UpdateDrawPile() {
        CardProspector cd;

        for (int i=0; i<drawPile.Count; i++)        {
            cd = drawPile[i];
            cd.transform.parent = layoutAnchor;

            Vector2 dpStagger = layout.drawPile.stagger;
            cd.transform.localPosition = new Vector3 (
                layout.multiplier.x * (layout.drawPile.x + i*dpStagger.x), 
                layout.multiplier.y * (layout.drawPile.y + i*dpStagger.y),
                -layout.drawPile.layerID + .1f * i);

            cd.faceUp = false;

            cd.state = eCardState.drawpile;

            cd.SetSortingLayerName(layout.drawPile.layerName);
            cd.SetSortOrder(-10 * i);
        }//for
    }//void

    public void CardClicked(CardProspector cd)    {
        switch (cd.state)        {
            case eCardState.target:
                break;
            case eCardState.drawpile:
                MoveToDiscard(target);
                MoveToTarget(Draw());
                UpdateDrawPile();
                break;
            case eCardState.tableau:
                bool validMatch = true;
                if(!cd.faceUp)                {
                    validMatch = false;
                }//if

                if (!AdjacentRank(cd, target))                {
                    validMatch = false;
                }//if

                if (!validMatch) return;

                tableau.Remove(cd);
                MoveToTarget(cd);
                SetTableauFaces();
                break;
        }//switch
        CheckForGameOver();
    }//public void

    void CheckForGameOver()    {
        if (tableau.Count == 0)        {
            GameOver(true);
            return;
        }//if

        if (drawPile.Count>0) {
            return;
        }//if

        foreach(CardProspector cd in tableau)        {
            if(AdjacentRank(cd, target))            {
                return;
            }//if
        }//foreach
        GameOver(false);
    }//void

    void GameOver (bool won)    {
        if (won)        {
            print("Game Over. You wont! :)");
        }//if
        else        {
            print("Gameover. You Lost. :(");
        }//else

        SceneManager.LoadScene("__Prospector_Scene_0");
           
    }//void


    public bool AdjacentRank(CardProspector c0, CardProspector c1)    {
        if (!c0.faceUp || !c1.faceUp) return (false);

        if (Mathf.Abs(c0.rank -c1.rank)==1)
        {
            return (true);
        }//if

        if (c0.rank == 1 && c1.rank == 13) return (true);
        if (c0.rank == 13 && c1.rank == 1) return (true);

        return (false);
    }//public bool

}
