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

	[Header("Set Dynamically")]
	public Deck					deck;
    public Layout layout;
    public List<CardProspector> drawPile;

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
    
}
