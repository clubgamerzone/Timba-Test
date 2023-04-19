using ClubGamerZone.Core;
using ClubGamerZone.Data.Remote;
using ClubGamerZone.Leaderboards;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubGamerZone
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public Dictionary<string, UserLeaderboard> CurrentUserList = new Dictionary<string, UserLeaderboard>();
        public string Username;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            DontDestroyOnLoad(gameObject);
        }

        public void UpdateUserList(Dictionary<string, UserLeaderboard> newDict)
        {
            CurrentUserList.Clear();
            CurrentUserList = newDict;
        }             

        public void PostScore( int score, Action OnSuccess = null, Action OnFailed = null)
        {
            UserLeaderboard lb = new UserLeaderboard
            {
                PlayerName = Username,
                Score = score,
            };

            string json = JsonConvert.SerializeObject(lb);


            foreach (var item in CurrentUserList)
            {
                if (item.Value.PlayerName == lb.PlayerName)
                {
                    if (item.Value.Score >= lb.Score) return;
                    print(lb.Score);
                    Rest.Instance.Patch(Rest.Instance.URL + Rest.Instance.LeaderBoardParms + item.Key + ".json", json, (x) =>
                    {
                        print("patched: " + Rest.Instance.URL + Rest.Instance.LeaderBoardParms + item.Key + ".json");
                    }, (x) =>
                    {
                        print("error: " + x.ToString());
                    });
                    OnSuccess?.Invoke();
                    return;
                }
            }

            Rest.Instance.Post(Rest.Instance.URL + Rest.Instance.LeaderBoardParms + ".json", json);
            OnSuccess?.Invoke();
        }

    }
}
