using System;
using Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerLifecycleHandler : MonoBehaviour
    {
        private Player _player;
        private PlayerData.Settings _playerData;
        private GameState _gameState;
        
        private int _previousScore;
        private int _getALifeEveryTenK = 10000;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

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
            _player.CurrentLives = new ReactiveProperty<int>(2);
        }

        private void Start()
        {
            _gameState.Score.Subscribe(currentScore =>
            {
                if (_previousScore < _getALifeEveryTenK && currentScore >= _getALifeEveryTenK)
                {  
                    _getALifeEveryTenK += 10000;
                    
                    _player.CurrentLives.Value++;
                }
                
                _previousScore = currentScore;
            });
        }

        private void OnTriggerEnter(Collider other)
        {
            _player.CurrentLives.Value--;

            Observable
                .Timer(TimeSpan.FromSeconds(_playerData.respawnDelay))
                .Subscribe(_ =>
                {
                    if (_player.IsDead && _player.CurrentLives.Value >= 0)
                    {
                        _player.MeshRenderer.enabled = true;
                        _player.MeshCollider.enabled = false;
                
                        _player.Facing = Vector3.zero;
                        _player.Position = new Vector3(0,1,0);
                        _player.Rotation = Vector3.up;
                
                        _player.JustRespawned.Value = true;
                        _player.IsDead = false;
                    }
                })
                .AddTo(_disposables);
        }
    }
}
