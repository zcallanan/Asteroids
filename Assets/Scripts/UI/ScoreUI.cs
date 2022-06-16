using Controllers;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private Text scoreText;

        private void Start()
        {
            GameManager.sharedInstance.OnScoreUpdate += HandleScoreUpdate;
            scoreText.text = "0";
        }

        private void HandleScoreUpdate(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}
