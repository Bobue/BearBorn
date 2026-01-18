using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public Suit suit;
    public int number;

    private int handIndex;
    private PokerGameManager manager;

    public void SetCard(Suit suit, int number)
    {
        this.suit = suit;
        this.number = number;
    }


    public Card ToCard()
    {
        return new Card(suit, number);
    }

    public void SetIndex(int index)
    {
        handIndex = index;
    }

    public void SetManager(PokerGameManager mgr)
    {
        manager = mgr;
    }

    private void OnMouseDown()
    {
        if (manager != null)
        {
            manager.ToggleCardSelection(handIndex);
        }
    }
}
