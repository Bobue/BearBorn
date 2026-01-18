using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchChekcing : MonoBehaviour
{
    public List<Tiles> tileList = new List<Tiles>();

    public List<C_TileType> CheckAllMatches()
    {
        tileList.Clear();

        Board board = FindObjectOfType<Board>();
        Tiles[,] tileMap = board.C_tilesMap;

        int width = board.width;
        int height = board.height;

        // ======================
        // 가로 매치 체크
        // ======================
        for (int x = 0; x < width - 2; x++)
        {
            for (int y = 0; y < height; y++)
            {
                C_TileType type = tileMap[x, y].TileType;

                if (tileMap[x + 1, y].TileType == type &&
                    tileMap[x + 2, y].TileType == type)
                {
                    MarkMatched(tileMap[x, y]);
                    MarkMatched(tileMap[x + 1, y]);
                    MarkMatched(tileMap[x + 2, y]);
                }
            }
        }

        // ======================
        // 세로 매치 체크
        // ======================
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 2; y++)
            {
                C_TileType type = tileMap[x, y].TileType;

                if (tileMap[x, y + 1].TileType == type &&
                    tileMap[x, y + 2].TileType == type)
                {
                    MarkMatched(tileMap[x, y]);
                    MarkMatched(tileMap[x, y + 1]);
                    MarkMatched(tileMap[x, y + 2]);
                }
            }
        }

        // 중복 타일 제거
        tileList = tileList.Distinct().ToList();

        // 매치된 타일 타입만 반환
        return tileList
            .Select(t => t.TileType)
            .Distinct()
            .ToList();
    }

    private void MarkMatched(Tiles tile)
    {
        if (!tile.isMatched)
        {
            tile.isMatched = true;
            tileList.Add(tile);
        }
    }
}
