using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TMP_Text text;

    public void Init(float damage, Color color)
    {
        text.text = damage.ToString("0");
        text.color = color;
        transform.DOMoveY(transform.position.y + 1f, 0.6f);
        text.DOFade(0, 0.6f);
        

        Destroy(gameObject, 0.6f);
    }
}

