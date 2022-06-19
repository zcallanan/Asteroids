using System.Collections;
using Controllers;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverUI : MonoBehaviour
    {
        private Text _gameOverText;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private void Start()
        {
            _gameOverText = gameObject.GetComponent<Text>();
            _gameOverText.enabled = false;
            
            GameManager.sharedInstance.GameOver
                .Subscribe(HandleGameOver)
                .AddTo(_disposables);
        }
        
        private void HandleGameOver(bool gameOver)
        {
            if (gameOver)
            {
                StartCoroutine(EnableGameOverTextCoroutine());
                _disposables.Clear();
            }
        }

        private IEnumerator EnableGameOverTextCoroutine()
        {
            yield return new WaitForSeconds(2f);
            _gameOverText.enabled = true;
        }
    }
}
