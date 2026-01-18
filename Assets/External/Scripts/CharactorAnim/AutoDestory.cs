using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestory : MonoBehaviour//애니메이션 자동 파괴 스크립트
{
    public float lifeTime = 0.5f;//파괴 시간

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
