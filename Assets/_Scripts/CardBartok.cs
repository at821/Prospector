using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CBState
{
    toDrawPile,
    drawpile,
    toHand,
    hand,
    toTarget,
    target,
    discard,
    to,
    idle
}
public class CardBartok : Card {

    static public float MOVE_DURATION = .5f;
    static public string MOVE_EASING = Easing.InOut;
    static public float CARD_HEIGHT = 3.5f;
    static public float CARD_WIDTH = 2f;

    [Header("Set Dynamically: CardBartok")]
    public CBState state = CBState.drawpile;
    public List<Vector3> bezierPts;
    public List<Quaternion> bezierRots;
    public float timeStart, timeDuration;
    public int eventualSortOrder;
    public string eventualSortLayer;
    

    public GameObject reportFinishTo = null;

    [System.NonSerialized]
    public Player callbackPlayer = null;

    public void MoveTo(Vector3 ePos, Quaternion eRot)
    {
        bezierPts = new List<Vector3>();
        bezierPts.Add(transform.localPosition);
        bezierPts.Add(ePos);

        bezierRots = new List<Quaternion>();
        bezierRots.Add(transform.rotation);
        bezierRots.Add(eRot);

        if (timeStart ==0)
        {
            timeStart = Time.time;
        }//if

        timeDuration = MOVE_DURATION;
        state = CBState.to;
        
    }//public void

    public void MoveTo(Vector3 ePos)
    {
        MoveTo(ePos, Quaternion.identity);
    }//public void

    void Update()
    {
        switch (state)
        {
            case CBState.toHand:
            case CBState.toTarget:
            case CBState.toDrawPile:
            case CBState.to:
                float u = (Time.time - timeStart) / timeDuration;
                float uC = Easing.Ease(u, MOVE_EASING);

                if (u < 0)
                {
                    transform.localPosition = bezierPts[0];
                    transform.rotation = bezierRots[0];
                    return;
                }//if
                else if (u >= 1)
                {
                    uC = 1;

                    if (state == CBState.toHand) state = CBState.hand;
                    if (state == CBState.toTarget) state = CBState.target;
                    if (state == CBState.toDrawPile) state = CBState.drawpile;
                    if (state == CBState.to) state = CBState.idle;

                    transform.localPosition = bezierPts[bezierPts.Count - 1];
                    transform.rotation = bezierRots[bezierPts.Count - 1];

                    timeStart = 0;

                    if (reportFinishTo != null)
                    {
                        reportFinishTo.SendMessage("CBCallback", this);
                        reportFinishTo = null;
                    }//if
                    else if (callbackPlayer != null)
                    {
                        callbackPlayer.CBCallback(this);
                        callbackPlayer = null;
                    }//else if
                    else
                    {
                        //empty in book
                    }//else

                }//else if
                else
                {
                    Vector3 pos = Utils.Bezier(uC, bezierPts);
                    transform.localPosition = pos;
                    Quaternion rotQ = Utils.Bezier(uC, bezierRots);
                    transform.rotation = rotQ;

                    if (u > .5f)
                    {
                        SpriteRenderer sRend = spriteRenderers[0];
                        if (sRend.sortingOrder != eventualSortOrder)
                        {
                            SetSortOrder(eventualSortOrder);
                        }//if
                        if (sRend.sortingLayerName != eventualSortLayer)
                        {
                            SetSortingLayerName(eventualSortLayer);
                        }
                    }//if


                }//else
                break;
        }//switch
    }//void

    override public void OnMouseUpAsButton()
    {
        Bartok.S.CardClicked(this);
        base.OnMouseUpAsButton();
    }//override
}
