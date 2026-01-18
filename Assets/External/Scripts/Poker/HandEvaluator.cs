using System.Collections.Generic;
using System.Linq;

public enum HandRank
{
    FourOfAKind,  // 포카드
    FullHouse,    // 풀하우스
    Flush,        // 플러쉬
    Straight,     // 스트레이트
    Triple, // 트리플
    TwoPair,      // 투페어
    OnePair,      // 원페어
    High      // 족보 없음
}

public static class HandEvaluator
{
    public static HandRank EvaluateHand(List<Card> hand)
    {
        if (hand == null || hand.Count == 0)
        {
            return HandRank.High;
        }
        var numbers = hand.Select(card => card.Number).ToList();
        var suits = hand.Select(card => card.Suit).ToList();

        var numberGroups = numbers.GroupBy(n => n).OrderByDescending(g => g.Count()).ToList();
        bool isFlush = suits.Distinct().Count() == 1;
        bool isStraight = IsStraight(numbers);

        if (numberGroups[0].Count() == 4) return HandRank.FourOfAKind;

        if (numberGroups[0].Count() == 3 && numberGroups.Count > 1 && numberGroups[1].Count() == 2)
            return HandRank.FullHouse;

        if (isFlush) return HandRank.Flush;

        if (isStraight) return HandRank.Straight;

        if (numberGroups[0].Count() == 3) return HandRank.Triple;

        if (numberGroups.Count(g => g.Count() == 2) == 2) return HandRank.TwoPair;

        if (numberGroups[0].Count() == 2) return HandRank.OnePair;

        return HandRank.High;
    }

    private static bool IsStraight(List<int> numbers)
    {
        var sorted = numbers.Distinct().OrderBy(n => n).ToList();

        if (sorted.Count < 5) return false;

        for (int i = 0; i <= sorted.Count - 5; i++)
        {
            bool straight = true;
            for (int j = 0; j < 4; j++)
            {
                if (sorted[i + j] + 1 != sorted[i + j + 1])
                {
                    straight = false;
                    break;
                }
            }
            if (straight) return true;
        }
        return false;
    }

    public static float GetDamageMultiplier(HandRank rank, List<Card> hand)
    {
        switch (rank)
        {
            case HandRank.FourOfAKind: return 1.00f;
            case HandRank.FullHouse: return 0.60f;
            case HandRank.Flush: return 0.50f;
            case HandRank.Straight: return 0.35f;
            case HandRank.Triple: return 0.25f;
            case HandRank.TwoPair: return 0.20f;
            case HandRank.OnePair: return 0.15f;
            case HandRank.High:
                int top = hand.Max(card => card.Number);
                return top switch
                { 
                    1 => 0.01f,
                    2 => 0.02f,
                    3 => 0.03f,
                    4 => 0.04f,
                    5 => 0.05f,
                    6 => 0.06f,
                    7 => 0.07f,
                    8 => 0.08f,
                    9 => 0.09f,
                    10 => 0.10f
                };
            default:
                return 0f;
        }
    }

}
