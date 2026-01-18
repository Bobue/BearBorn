using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PokerGameManager : MonoBehaviour
{
    public List<GameObject> cardPrefabs; // 40장 카드 프리팹들
    public TMP_Text resultText;



    private List<GameObject> deck = new List<GameObject>();
    public List<GameObject> hand = new List<GameObject>();

    private List<int> selectedIndices = new List<int>(); // 선택된 카드 인덱스 리스트
    private int nextDeckIndex; // 덱에서 다음에 뽑을 카드 인덱스

    private int cardChangeCount;
    private bool ischanced;

    // 카드 위치 X 좌표 (5장 기준)
    private float[] xPositions = { -2.2f, -1.1f, 0f, 1.1f, 2.2f };
    
    public event System.Action OnGameEnd;

    void Start()
    {
        StageManager.Instance.StartPokerTurn += StartPokerTurn;
        InitDeck();
        ShuffleDeck();
    }

    public void EndGame()
    {
        MySoundManager.Instance.PlayCardSelect();
        OnGameEnd?.Invoke();
    }

    void InitDeck()
    {
        Transform cardCreatTransform = this.transform.Find("PokerCards");
        if (!cardCreatTransform)
        {
            GameObject newObj = new GameObject("PokerCards");
            newObj.transform.SetParent(this.transform, false); 
            cardCreatTransform = newObj.transform;
        }

        deck.Clear();
        foreach (var prefab in cardPrefabs)
        {
            GameObject card = Instantiate(prefab, cardCreatTransform);
            card.SetActive(false);
            deck.Add(card);
        }
        nextDeckIndex = 5; // 처음 5장 뽑았으니 다음 카드는 5번째 인덱스부터
    }

    void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var temp = deck[k];
            deck[k] = deck[n];
            deck[n] = temp;
        }
    }

    void DrawHand(int count)
    {
        hand.Clear();
        for (int i = 0; i < count && i < deck.Count; i++)
        {
            hand.Add(deck[i]);
        }
    }


    void ShowHand()
    {
        for (int i = 0; i < hand.Count && i < xPositions.Length; i++)
        {
            hand[i].SetActive(true);
            hand[i].transform.position = new Vector3(xPositions[i], 3, 0);

            CardDisplay cardDisplay = hand[i].GetComponent<CardDisplay>();
            if (cardDisplay != null)
            {
                cardDisplay.SetIndex(i);
                cardDisplay.SetManager(this);
                UpdateCardHighlight(i, false); // 처음에는 선택 해제 상태로
            }
        }
    }

    // 카드 선택 
    public void ToggleCardSelection(int index)
    {
        // 교체 횟수 없으면 선택/해제 불가
        if (cardChangeCount <= 0 || !ischanced)
            return;

        if (selectedIndices.Contains(index))
        {
            selectedIndices.Remove(index);
            UpdateCardHighlight(index, false);
            MySoundManager.Instance.PlayCardSelect();
        }
        else
        {
            if (selectedIndices.Count >= 2)
            {
                Debug.Log("최대 2장까지만 선택 가능합니다.");
                return;
            }
            selectedIndices.Add(index);
            UpdateCardHighlight(index, true);
            MySoundManager.Instance.PlayCardSelect();
        }
    }


    // 카드 선택 시 색상 변경
    void UpdateCardHighlight(int index, bool selected)
    {
        if(ischanced)
        {
            if (index < 0 || index >= hand.Count) return;

            SpriteRenderer sr = hand[index].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = selected ? new Color32(0x80, 0x80, 0x80, 0xFF) : Color.white;
            }
        }
    }

    // 선택된 카드들 한꺼번에 교체
    public void ReplaceSelectedCards()
    {
        if(cardChangeCount <=  0)
        {
            ischanced = false;
            Debug.Log("교체 가능 횟수를 모두 소진했습니다.");
            return;
        }

        if (selectedIndices.Count == 0)
        {
            Debug.Log("교체할 카드를 선택하세요.");
            return;
        }

        foreach (int index in selectedIndices)
        {
            ReplaceCard(index);
            UpdateCardHighlight(index, false);
        }

        --cardChangeCount;
        selectedIndices.Clear();

        EvaluateAndShowRank();
    }

    // 카드 한 장 교체 (ReplaceSelectedCards에서 호출)
    private void ReplaceCard(int handIndex)
    {
        if (handIndex < 0 || handIndex >= hand.Count)
        {
            Debug.LogWarning("잘못된 카드 인덱스");
            return;
        }

        if (nextDeckIndex >= deck.Count)
        {
            Debug.Log("덱에 교체할 카드가 없음");
            return;
        }

        GameObject oldCard = hand[handIndex];
        oldCard.SetActive(false);

        GameObject newCard = deck[nextDeckIndex];
        nextDeckIndex++;

        hand[handIndex] = newCard;
        newCard.SetActive(true);
        newCard.transform.position = new Vector3(xPositions[handIndex], 3, 0);

        CardDisplay newDisplay = newCard.GetComponent<CardDisplay>();
        if (newDisplay != null)
        {
            newDisplay.SetIndex(handIndex);
            newDisplay.SetManager(this);
        }
    }

    public float EvaluateAndShowRank()
    {
        List<Card> cardData = new List<Card>();
        foreach (var cardObj in hand)
        {
            CardDisplay display = cardObj.GetComponent<CardDisplay>();
            if (display != null)
            {
                cardData.Add(display.ToCard());
            }
        }

        HandRank rank = HandEvaluator.EvaluateHand(cardData);
        float multiplier = HandEvaluator.GetDamageMultiplier(rank, cardData);

        string rankText;

        if (rank == HandRank.High)
        {
            int top = cardData.Max(card => card.Number);
            rankText = $"{top} High";
        }
        else
        {
            rankText = rank.ToString();
        }

        resultText.text = $"족보: {rankText}\n배수: {multiplier:F4}";
        Debug.Log(resultText.text);

        return multiplier;
    }

    private void StartPokerTurn()
    {
        foreach (var card in deck)
            card.SetActive(false);
        ShuffleDeck();  

        selectedIndices.Clear();
        cardChangeCount = 2;
        ischanced = true;

        DrawHand(5);
        ShowHand();
        //EvaluateAndShowRank();
    }
}