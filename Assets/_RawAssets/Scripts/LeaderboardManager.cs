using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager manager;

    public LB_Details lB_DetailsPrefab;
    public Transform leaderboardTransform;
    public GameObject leaderboardMenu;
    

    public List<PlayerData> list_playerData = new List<PlayerData>();

    [System.Serializable]
    public class PlayerData
    {
        public int number;
        public string name;
        public int level;
        public Texture2D profileTexture; // optional, for future avatar support
    }

    private void Awake()
    {
        manager = this;
    }

    /// <summary>
    /// Call this to submit the current player's level as their leaderboard score.
    /// </summary>
    public void SendLeaderboard(int score)
    {
        PlayFabManager.Instance.SendLeaderBoard(score);
    }

    /// <summary>
    /// Fetches leaderboard data from PlayFab then renders the UI.
    /// </summary>
    public void ShowLeaderboard()
    {
        PlayFabManager.Instance.GetLeaderboard(OnLeaderboardDataReceived);
    }

    private void OnLeaderboardDataReceived(List<PlayerData> data)
    {
        list_playerData = data;
        ShowLeaderboardMenu();
    }

    private void ShowLeaderboardMenu()
    {
        // Clear old entries
        for (int i = 0; i < leaderboardTransform.childCount; i++)
        {
            Destroy(leaderboardTransform.GetChild(i).gameObject);
        }

        foreach (var item in list_playerData)
        {
            LB_Details lB_Details = Instantiate(lB_DetailsPrefab, leaderboardTransform);
            lB_Details.text_rank.text = item.number.ToString();
            lB_Details.text_name.text = item.name;
            lB_Details.text_level.text = item.level.ToString();
        }

        leaderboardMenu.SetActive(true);
    }

    public void HideLeaderboard()
    {
        leaderboardMenu.SetActive(false);
    }
}