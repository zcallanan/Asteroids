using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Misc
{
    public static class AsteroidSpawnLocation
    {
        private static Vector3 _minBounds;
        private static Vector3 _maxBounds;
        private static Vector3 _innerMinBounds;
        private static Vector3 _innerMaxBounds;

        public static Vector3 DetermineLargeAsteroidsPosition(Vector3 minBounds, Vector3 maxBounds, Vector3 innerMinBounds,
            Vector3 innerMaxBounds)
        {
            _minBounds = minBounds;
            _maxBounds = maxBounds;
            _innerMinBounds = innerMinBounds;
            _innerMaxBounds = innerMaxBounds;
            
            var side = Random.Range(0, 4);
            var positionResult = side switch
            {
                0 => SpawnLocationAtTopOfScreen(),
                1 => SpawnLocationAtRightOfScreen(),
                2 => SpawnLocationAtBottomOfScreen(),
                3 => SpawnLocationAtLeftOfScreen(),
                _ => throw new ArgumentOutOfRangeException(nameof(side), $"Not expected side value: {side}"),
            };
            return positionResult;
        }

        private static Vector3 SpawnLocationAtTopOfScreen()
        {
            return new Vector3(Random.Range(_minBounds.x, _maxBounds.x), 1.0f,
                Random.Range(_innerMaxBounds.z, _maxBounds.z));
        }

        private static Vector3 SpawnLocationAtRightOfScreen()
        {
            return new Vector3(Random.Range(_innerMaxBounds.x, _maxBounds.x), 1.0f,
                Random.Range(_minBounds.z, _maxBounds.z));
        }

        private static Vector3 SpawnLocationAtBottomOfScreen()
        {
            return new Vector3(Random.Range(_minBounds.x, _maxBounds.x), 1.0f,
                Random.Range(_innerMinBounds.z, _minBounds.z));
        }

        private static Vector3 SpawnLocationAtLeftOfScreen()
        {
            return new Vector3(Random.Range(_innerMinBounds.x, _minBounds.x), 1.0f,
                Random.Range(_minBounds.z, _maxBounds.z));
        }
    }
}
