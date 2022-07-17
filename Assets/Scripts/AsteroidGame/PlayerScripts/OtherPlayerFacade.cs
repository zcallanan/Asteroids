using AsteroidGame.Misc;
using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class OtherPlayerFacade : MonoBehaviour
    {
        private Player _player;
        private PlayerLifecycleHandler _playerLifecycleHandler;
        private GameState _gameState;

        public Vector3 Position => _player.Position;

        public bool CanCollide => _player.CanCollide;

        [Inject]
        public void Construct(
            Player player,
            PlayerLifecycleHandler playerLifecycleHandler,
            GameState gameState)
        {
            _player = player;
            _playerLifecycleHandler = playerLifecycleHandler;
            _gameState = gameState;
        }

        private void Awake()
        {
            _player.HyperspaceWasTriggered = new ReactiveProperty<bool>(false);
            _player.JustRespawned = new ReactiveProperty<bool>(false);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_player.CanCollide)
            {
                if (other.GetComponent<PlayerFacade>())
                {
                    var playerFacade = other.GetComponent<PlayerFacade>();
                    
                    if (playerFacade.CanCollide)
                    {
                        _playerLifecycleHandler.PlayerDeathEvents();
                    }
                } 
                else if (other.GetComponent<BulletProjectile>())
                {
                    var colliderObjectType = other.GetComponent<BulletProjectile>().OriginType;
                
                    if (colliderObjectType != ObjectTypes.OtherPlayer)
                    {
                        _playerLifecycleHandler.PlayerDeathEvents();
                    }
                }
                else
                {
                    _playerLifecycleHandler.PlayerDeathEvents();
                }
            }
        }
    }
}
