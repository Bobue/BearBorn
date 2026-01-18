using System.Collections.Generic;
using UnityEngine;

public class TestVerka : MonoBehaviour
{
    Verka verka = new Verka();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PokerGameManager manager = FindObjectOfType<PokerGameManager>();
            if (manager == null)
            {
                Debug.LogWarning("PokerGameManager를 찾을 수 없습니다.");
                return;
            }

            List<GameObject> hand = manager.hand; 

            if (hand == null || hand.Count != 5)
            {
                Debug.LogWarning("현재 손패가 유효하지 않습니다.");
                return;
            }

            List<Card> cardData = new List<Card>();
            foreach (GameObject cardObj in hand)
            {
                CardDisplay display = cardObj.GetComponent<CardDisplay>();
                if (display != null)
                {
                    cardData.Add(display.ToCard());
                }
            }

            if (cardData.Count != 5)
            {
                Debug.LogWarning("카드 데이터가 5장이 아닙니다.");
                return;
            }

            HandRank rank = HandEvaluator.EvaluateHand(cardData);
            float finalDamage = verka.CalculateFinalDamage(rank, cardData);

            Debug.Log($"[TestVerka] 족보: {rank}, 데미지: {finalDamage:F2}");
        }
    }
}
