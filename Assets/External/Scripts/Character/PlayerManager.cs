using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public event Action OnAllAllysDead;

    public List<PlayerCharacter> playerCharacters = new List<PlayerCharacter>();

    void Awake()
    {
        Instance = this;
    }

    public PlayerCharacter GetCharacterByTileType(C_TileType type)
    {
        Debug.Log("Player count: " + PlayerManager.Instance.playerCharacters.Count);

        switch (type)
        {
            case C_TileType.Verka:
                return playerCharacters.Find(p => p.name == "Verka");
            case C_TileType.Chirr:
                return playerCharacters.Find(p => p.name == "Chirr");
            case C_TileType.Marshika:
                return playerCharacters.Find(p => p.name == "Marshika");
            case C_TileType.Normal:
                return playerCharacters.Find(p => p.name == "Normal");
            default:
                return null;
        }
    }

    public void AddPlayer(PlayerCharacter player)
    {
        if (!playerCharacters.Contains(player))
            playerCharacters.Add(player);
    }
    public void RemovePlayer(PlayerCharacter player)
    {
        if (playerCharacters.Contains(player))
            playerCharacters.Remove(player);

        CheckAllPlayersDead();
    }

    private void CheckAllPlayersDead()
    {
        if (playerCharacters.Count == 0)
        {
            Debug.Log("아군 다 죽음.");
        }
    }
    public void ClearAllPlayers()
    {
        foreach (var p in playerCharacters)
        {
            if (p != null)
                Destroy(p.gameObject);
        }

        playerCharacters.Clear();

    }
}

