// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System;

// public class LeaderboardManager : MonoBehaviour
// {
//     public static LeaderboardManager manager;

//     public PlayFabManager playFabManager;
//     public List<PlayerData> list_playerData = new List<PlayerData>();
//     public LB_Details lB_DetailsPrefab;
//     public Transform leaderboardTransform;
//     public GameObject leaderboardMenu;


//     [System.Serializable]
//     public class PlayerData
//     {
//         public int number;
//         public string name;
//         public int level;
//     }

//     private void Awake() { manager = this; }

//     private void Start()
//     {

//     }

//     public void SendLeaderboard(int score)
//     {
//         playFabManager.SendLeaderBoard(score);
//     }

//     public void ShowLeaderboard()
//     {
//         playFabManager.GetLeaderboard();
//     }

//     public void ShowLeaderboardMenu()
//     {
//         for (int i = 0; i < leaderboardTransform.childCount; i++)
//         {
//             Destroy(leaderboardTransform.GetChild(i).gameObject);
//         }
//         foreach (var item in list_playerData)
//         {
//             LB_Details lB_Details = Instantiate(lB_DetailsPrefab, leaderboardTransform);
//             lB_Details.text_rank.text = "" + item.number;
//             lB_Details.text_name.text = "" + item.name;
//             lB_Details.text_level.text = "" + item.level;
//         }
//         leaderboardMenu.SetActive(true);
//     }
// }
