using ProjectScripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace AsteroidGame.PlayerScripts
{
    public class PlayerFacade : MonoBehaviour
    {
        private Player _player;

        public Vector3 Position => _player.Position;

        [Inject]
        public void Construct(
            Player player)
        {
            _player = player;
        }

        private void Awake()
        {
            _player.HyperspaceWasTriggered = new ReactiveProperty<bool>(false);
        }
    }
}
