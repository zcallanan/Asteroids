using AsteroidScripts;
using PlayerScripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class ExplosionHandler : MonoBehaviour
    {
        private PlayerFacade _playerFacade;

        [Inject]
        public void Construct(PlayerFacade playerFacade)
        {
            _playerFacade = playerFacade;
        }

        public void Start()
        {
            _playerFacade.gameObject
                .GetComponent<ObservableTriggerTrigger>()
                .OnTriggerEnterAsObservable()
                .Subscribe(_ => Debug.Log("Hello from EH"));
        }
    }
}
