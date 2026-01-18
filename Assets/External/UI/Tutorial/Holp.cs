using UnityEngine;

public class Holp : MonoBehaviour
{
    public GameObject[] Helps;

    // 버튼에서 index 넘겨서 호출
    public void ShowHelp(int index)
    {
        // 안전 체크
        if (Helps == null || index < 0 || index >= Helps.Length)
            return;

        // 전부 끄고
        for (int i = 0; i < Helps.Length; i++)
        {
            if (Helps[i] != null)
                Helps[i].SetActive(false);
        }

        // 선택한 것만 켜기
        Helps[index].SetActive(true);
    }
}
