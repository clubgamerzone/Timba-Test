using ClubGamerZone.Data.Remote;
using ClubGamerZone.Leaderboards;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace ClubGamerZone.Core
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _play;
        [SerializeField] private Button _leaderboards;
        [SerializeField] private GameObject _leaderboardItem;
        [SerializeField] private Transform _itemHolder;
        [SerializeField] private TMP_InputField _userName;


        private void Start()
        {
            _play.onClick.AddListener(OnPlayClicked);
            _leaderboards.onClick.AddListener(OnLeaderboardsClicked);
            _leaderboards.onClick.Invoke();
        }

        private void OnLeaderboardsClicked()
        {
            GetLeaderboards();
        }

        private void OnPlayClicked()
        {
            if (string.IsNullOrEmpty(_userName.text)) return;

            GameManager.Instance.Username = _userName.text;
            SceneManager.LoadScene(1);
        }

        public void GetLeaderboards()
        {
            Rest.Instance.Get(Rest.Instance.URL + Rest.Instance.LeaderBoardParms + ".json", (onSuccessResponse) =>
            {
                GetDataFromResponseString(onSuccessResponse);
            }, (onFailureResponse) =>
            {
                Debug.Log("<color=Red>FAILED TO RECEIVE DATABASE:</color>\n" + onFailureResponse.ToString());
            });
        }
        private void GetDataFromResponseString(string jsonRaw)
        {
            var leaderboardItems = JsonConvert.DeserializeObject<Dictionary<string, UserLeaderboard>>(jsonRaw);
            OrganizeRankingData(leaderboardItems);
        }
        private void OrganizeRankingData(Dictionary<string, UserLeaderboard> myDict)
        {
            if (myDict == null) return;
            var curLb = _itemHolder.transform.GetComponentsInChildren<PlayerLeaderboardItem>();
            foreach (var item in curLb)
            {
                Destroy(item.gameObject);
            }
            var sortedDict = from entry in myDict orderby entry.Value.Score descending select entry;
            foreach (var item in sortedDict)
            {
                GameObject playerLeaderboard = Instantiate(_leaderboardItem);
                playerLeaderboard.GetComponent<PlayerLeaderboardItem>().UpdateFields(item.Value.PlayerName, item.Value.Score);
                playerLeaderboard.transform.SetParent(_itemHolder.transform, false);
                playerLeaderboard.transform.localScale = Vector2.one;
            }
            GameManager.Instance.UpdateUserList(myDict);
        }
    }
}
