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

            MoveToTarget(Draw());
            UpdateDrawPile();
        }//foreach
    }//layout
    
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
            cd.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.drawPile.x + i*dpStagger.x), 
                layout.multiplier.y * (layout.drawPile.y + i*dpStagger.y),
                -layout.drawPile.layerID + .1f * i);

            cd.faceUp = false;

            cd.state = eCardState.drawpile;

            cd.SetSortingLayerName(layout.drawPile.layerName);
            cd.SetSortOrder(-10 * i);
        }//for
    }//void

    public void CardClicked(CardProspector cd)
    {
        switch (cd.state)
        {
            case eCardState.target:
                break;
            case eCardState.drawpile:
                MoveToDiscard(target);
                MoveToTarget(Draw());
                UpdateDrawPile();
                break;
            case eCardState.tableau:
                break;
        }//switch
    }//public void

}
