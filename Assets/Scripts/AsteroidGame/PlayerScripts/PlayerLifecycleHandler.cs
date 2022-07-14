using System;
using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerLifecycleHandler : MonoBehaviour
    {
        private Player _player;
        private PlayerData.Settings _playerData;
        private GameState _gameState;
        
        private int _previousScore;
        private int _getALifeEveryTenK = 10000;

        private bool _gameIsOver;
        
        [Inject]
        public void Construct(
            Player player,
            PlayerData.Settings playerData,
            GameState gameState)
        {
            _player = player;
            _playerData = playerData;
            _gameState = gameState;
        }

        private void Awake()
        {
            _player.JustRespawned = new ReactiveProperty<bool>(false);
        }

        private void Start()
        {
            IncrementCurrentLivesEveryTenKScoreUnlessGameIsOver();
        }

        private void IncrementCurrentLivesEveryTenKScoreUnlessGameIsOver()
        {
            _gameState.Score.Subscribe(currentScore =>
            {
                if (_previousScore < _getALifeEveryTenK && currentScore >= _getALifeEveryTenK && !_gameIsOver)
                {  
                    _getALifeEveryTenK += 10000;
                    
                    _gameState.CurrentLives.Value++;
                }
                
                _previousScore = currentScore;
            });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<BulletProjectile>())
            {
                var colliderObjectType = other.GetComponent<BulletProjectile>().OriginType;
                
                if (colliderObjectType != ObjectTypes.Player)
                {
                    PlayerDeathEvents();
                }
            }
            else
            {
                PlayerDeathEvents();
            }
        }

        private void PlayerDeathEvents()
        {
            if (_gameState.CurrentLives.Value > -1)
            {
                _gameState.CurrentLives.Value--;
            }

            RestorePlayerFromDeathAfterDelay();

            DisablePlayerObjectOnDeath();

            if (!_gameIsOver)
            {
                SetGameOver();
            }
        }

        private void SetGameOver()
        {
            _gameState.CurrentLives
                .Subscribe(lives =>
                {
                    if (lives < 0)
                    {
                        _gameIsOver = true;
                        
                        Observable.Timer(TimeSpan.FromSeconds(7))
                            .Subscribe(_ =>
                            {
                                _gameState.IsGameRunning.Value = false;
                                _gameState.IsGameReset.Value = true;
                            })
                            .AddTo(_player.GameObj);
                    }
                })
                .AddTo(_player.GameObj);
        }
        
        private void DisablePlayerObjectOnDeath()
        {
            if (_gameState.CurrentLives.Value < 0)
            {
                _player.GameObj.SetActive(false);
            }
        }
        
        private void RestorePlayerFromDeathAfterDelay()
        {
            Observable
                .Timer(TimeSpan.FromSeconds(_playerData.respawnDelay))
                .Subscribe(_ =>
                {
                    if (_player.IsDead && _gameState.CurrentLives.Value >= 0)
                    {
                        _player.MeshRenderer.enabled = true;
                        _player.MeshCollider.enabled = false;
                
                        _player.Facing = Vector3.forward;
                        _player.Position = Vector3.up;
                        _player.SetRotation(Vector3.up);
                
                        _player.JustRespawned.Value = true;
                        _player.IsDead = false;
                    }
                })
                .AddTo(_player.GameObj);
        }
    }
}
