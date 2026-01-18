using UnityEngine;
using UnityEngine.UI;

public class TileselectManager : MonoBehaviour
{
    // 이벤트 선언: 선택된 타일 타입과 함께 알림
    public event System.Action<C_TileType> OnTileSelectEnd;

    public Button arrowButton;
    public Button shieldButton;
    public Button startButton;

    public GameObject arrowPrefab;
    public GameObject shieldPrefab;

    public GameObject arrowPanel;
    public GameObject shieldPanel;


    private C_TileType selectedTile = C_TileType.Normal;

    private Color defaultColor = Color.white;
    private Color selectedColor = Color.gray;

    private void Start()
    {
        arrowButton.onClick.AddListener(() => SelectTile(C_TileType.Arrow));
        shieldButton.onClick.AddListener(() => SelectTile(C_TileType.Shield));
        startButton.onClick.AddListener(() => EndTileSelect());

        arrowButton.image.color = defaultColor;
        shieldButton.image.color = defaultColor;

        if(arrowPanel) arrowPanel.SetActive(false);
        if(shieldPanel) shieldPanel.SetActive(false);
    }

    private void SelectTile(C_TileType type)
    {
        selectedTile = type;
        Debug.Log($"{selectedTile} 타일이 선택되었습니다!");
        arrowButton.image.color = (type == C_TileType.Arrow) ? selectedColor : defaultColor;
        shieldButton.image.color = (type == C_TileType.Shield) ? selectedColor : defaultColor;

        if (arrowPanel) arrowPanel.SetActive(type == C_TileType.Arrow);
        if (shieldPanel) shieldPanel.SetActive(type == C_TileType.Shield);

        MySoundManager.Instance?.PlayArrowTileSelect();
    }

    private void EndTileSelect()
    {
        if (selectedTile == C_TileType.Normal)
        {
            Debug.LogWarning("타일을 선택하세요!");
            return;
        }

        Debug.Log($"타일 선택 완료: {selectedTile}");
        MySoundManager.Instance?.PlayArrowTileSelect();
        // StageManager로 알림 (이벤트 발생)
        OnTileSelectEnd?.Invoke(selectedTile);

        // 선택 초기화
        selectedTile = C_TileType.Normal;
        arrowButton.image.color = defaultColor;
        shieldButton.image.color = defaultColor;

        if (arrowPanel) arrowPanel.SetActive(false);
        if (shieldPanel) shieldPanel.SetActive(false);
    }
}

