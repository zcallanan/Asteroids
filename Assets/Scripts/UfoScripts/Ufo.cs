using System;
using Misc;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace UfoScripts
{
    public class Ufo : MonoBehaviour, IPoolable<ObjectTypes, IMemoryPool>, IDisposable
    {
        private Difficulty.Settings _difficultySettings;
        private GameState _gameState;
        private BoundHandler _boundHandler;

        private float _ufoMinSpeed;
        private float _ufoMaxSpeed;
        
        public ObjectTypes Size { get; set; }
        
        public bool IsRecentlySpawned { get; set; }
        
        public Vector3 Facing { get; set; }
        
        private IMemoryPool _pool;

        [Inject]
        public void Construct(
            Difficulty.Settings difficultySettings,
            GameState gameState,
            BoundHandler boundHandler)
        {
            _difficultySettings = difficultySettings;
            _gameState = gameState;
            _boundHandler = boundHandler;
        }

        private void Start()
        {
            var difficulties = _difficultySettings.difficulties[_gameState.GameDifficulty];

            _ufoMinSpeed = difficulties.ufoMinSpeed;
            _ufoMaxSpeed = difficulties.ufoMaxSpeed;
        }

        private void Update()
        {
            MoveUfoForward();
        }

        private void OnTriggerEnter(Collider other)
        {
            Dispose();
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(ObjectTypes size, IMemoryPool pool)
        {
            Size = size;
            _pool = pool;
        }

        public void Dispose()
        { 
            _pool.Despawn(this);
        }
        
        public class Factory : PlaceholderFactory<ObjectTypes, Ufo>
        {
        }
        
        private void MoveUfoForward()
        {
            var position = transform.position;
            position +=
                Facing * (Time.deltaTime * Random.Range(_ufoMinSpeed, _ufoMaxSpeed));
             
            if (!IsRecentlySpawned)
            {
                position = _boundHandler.EnforceBounds(position);
            }
             
            transform.position = position;
        }
    }
}
