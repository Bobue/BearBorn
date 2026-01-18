using System;

public enum Suit
{
    Marshka, // 초록
    Chirr,   // 빨강
    Verka,   // 파랑
    Ether    // 회색
}

public class Card
{
    public Suit Suit { get; private set; }
    public int Number { get; private set; }

    public Card(Suit suit, int number)
    {
        if (number < 1 || number > 10)
            throw new ArgumentOutOfRangeException("number", "카드 숫자는 1~10까지임");
        Suit = suit;
        Number = number;
    }

    public override string ToString()
    {
        return $"{Suit} : {Number}";
    }
}
