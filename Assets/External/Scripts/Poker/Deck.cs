using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> cards;
    private System.Random rng = new System.Random();

    public Deck()
    {
        cards = new List<Card>();
        GenerateDeck();
        Shuffle();
    }

    private void GenerateDeck()
    {
        cards.Clear();
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            for (int number = 1; number <= 10; number++)
            {
                cards.Add(new Card(suit, number));
            }
        }
    }

    public void Shuffle()
    {
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card temp = cards[k];
            cards[k] = cards[n];
            cards[n] = temp;
        }
    }

    public Card DrawCard()
    {
        if (cards.Count == 0)
        {
            Debug.LogWarning("Deck is empty!");
            return null;
        }

        Card drawn = cards[0];
        cards.RemoveAt(0);
        return drawn;
    }

    public int CardsRemaining()
    {
        return cards.Count;
    }
}
