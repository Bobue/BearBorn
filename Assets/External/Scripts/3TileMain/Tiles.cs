using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum C_TileType
{
    Chirr,
    Verka,
    Marshika,
    Normal,
    Arrow,
    Shield
}

public class Tiles : MonoBehaviour
{
    public C_TileType TileType;
    public int x;
    public int y;

    public bool isMatched;
    private int blinkCount = 1;
    private float blinkDuration = 0.1f;
    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.isMatched = false;
    }
    public void Remove()
    {
        StartCoroutine("BlinkRoutine");
        Destroy(gameObject, blinkCount * blinkDuration * 2f);
    }
    IEnumerator BlinkRoutine()
    {
        Renderer renderer = GetComponent<Renderer>();
        for (int i = 0; i < blinkCount; i++)
        {
            renderer.enabled = false;
            yield return new WaitForSeconds(blinkDuration);
            renderer.enabled = true;
            yield return new WaitForSeconds(blinkDuration);
        }
    }
}
