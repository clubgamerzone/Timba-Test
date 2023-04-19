using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace ClubGamerZone.Core
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Button _clickable;
        private float _gameTime = 5;
        private float _countDown;
        private int _clicks;
        [SerializeField] private GameObject _gameFinished;
        [SerializeField] private Button _goToMainScreen;
        [SerializeField] private TextMeshProUGUI _totalClick;
        public static Action OnGameFinished;
        private bool _isGameFinished;
        private void OnEnable()
        {
            OnGameFinished += () => GameManager.Instance.PostScore(_clicks, OnPostScoreSuccess);
        }
        private void OnDisable()
        {
            OnGameFinished -= () => GameManager.Instance.PostScore(_clicks, OnPostScoreSuccess);
        }
        private void Start()
        {
            _isGameFinished = false;
            _countDown = _gameTime;
            _clickable.onClick.AddListener(SmashButton);
        }
        private void Update()
        {
            _countDown -= Time.deltaTime;
            if (_countDown <= 0 && !_isGameFinished)
            {
                _isGameFinished = true;
                _clickable.interactable = false;
                //OnGameFinished?.Invoke();
                GameManager.Instance.PostScore(_clicks, OnPostScoreSuccess);
            }
        }
        private void SmashButton()
        {
            _clicks++;
        }
        private void OnPostScoreSuccess()
        {
            if (!_gameFinished)
                _gameFinished = transform.GetChild(2).gameObject;
            _gameFinished.SetActive(true);
            _totalClick.text = _clicks + " times!";
            _goToMainScreen.onClick.RemoveAllListeners();
            _goToMainScreen.onClick.AddListener(GoToMain);
        }

        private void GoToMain()
        {
            SceneManager.LoadScene(sceneBuildIndex: 0);
        }
    }
}
