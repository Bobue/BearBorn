using UnityEngine;

public class DeckTester : MonoBehaviour
{
    private Deck deck;

    void Start()
    {
        deck = new Deck();

        for (int i = 0; i < 5; i++)
        {
            Card card = deck.DrawCard();
            if (card != null)
                Debug.Log(card.ToString());
        }

        Debug.Log("Remaining cards: " + deck.CardsRemaining());
    }
}
