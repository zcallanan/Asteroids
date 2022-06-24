// using Controllers;
// using UniRx;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace UI
// {
//     public class ScoreUI : MonoBehaviour
//     {
//         [SerializeField] private Text scoreText;
//         
//         private readonly CompositeDisposable _disposables = new CompositeDisposable();
//
//         private void Start()
//         {
//             GameManager.sharedInstance.IsGameOver.Subscribe(HandleGameOver).AddTo(_disposables);
//             
//             GameManager.sharedInstance.Score
//                 .Subscribe(HandleScoreUpdate)
//                 .AddTo(_disposables);
//             
//             scoreText.text = "0";
//         }
//
//         private void HandleScoreUpdate(int score)
//         {
//             scoreText.text = score.ToString();
//         }
//         
//         private void HandleGameOver(bool isGameOver)
//         {
//             if (isGameOver)
//             {
//                 _disposables.Clear();
//             }
//         }
//     }
// }
