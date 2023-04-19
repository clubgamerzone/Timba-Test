using TMPro;
using UnityEngine;

namespace ClubGamerZone.Leaderboards
{
    public class PlayerLeaderboardItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _score;

        // Update is called once per frame
        public void UpdateFields(string playerName, int playerScore)
        {
            _playerName.text = playerName;
            _score.text = playerScore.ToString();
        }
    }
}
