using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentSceneCameraInitializer : MonoBehaviour
{
    public float contentHeight = 10f;//콘텐츠 월드 높이(한 맵당 위아래 크기)
    void Start()
    {
        int selectedIndex = WorldMapManager.selectedContentIndex;

        if (selectedIndex != -1)
        {
            float targetY = selectedIndex * -contentHeight; //y값변환
            Debug.Log("contentHeight : "+contentHeight);
            Transform camTransform = this.transform;
            camTransform.position = new Vector3(camTransform.position.x, targetY, camTransform.position.z);
            Debug.Log((selectedIndex+1) + "번으로 이동했습니다");

            WorldMapManager.selectedContentIndex = -1;//인덱스 초기화
        }
        else
            Debug.Log("어라라 인덱스가 없어요");
    }
}
