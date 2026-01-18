using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int health = 100;
    public int damage = 20;

    public void Attack(Unit target)
    {
        Debug.Log($"{unitName}이(가) {target.unitName}을(를) 공격합니다!");
        target.TakeDamage(damage);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"{unitName}이(가) {amount}의 피해를 입었습니다. 남은 체력: {health}");

        if (health <= 0)
        {
            Debug.Log($"{unitName}이(가) 쓰러졌습니다!");
            Destroy(gameObject);
        }
    }
}
    