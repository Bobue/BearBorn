using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingData : MonoBehaviour//로딩후 불러올 씬 저장하는 클래스
{
    public static string TargetScene;

    public static void Clear()
    {
        TargetScene = null;
    }
}
