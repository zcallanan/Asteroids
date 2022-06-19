using UniRx;
using UnityEngine;

namespace Controllers
{
    public class BoundManager : MonoBehaviour
    {
        public static BoundManager sharedInstance;

        public Vector3 MaxBounds {get; private set;  }
        public Vector3 MinBounds { get; private set; }
        
        private static float _camDistance;
        private static Vector3 _bottomCorner;
        private static Vector3 _topCorner;
        private Camera _mainCamera;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private void Awake()
        {
            _mainCamera = Camera.main;
            sharedInstance = this;
            SetScreenBounds();
            
            GameManager.sharedInstance.GameOver
                .Subscribe(HandleGameOver)
                .AddTo(_disposables);
            
            GameManager.sharedInstance.LatestScreenSize
                .Subscribe(unit => SetScreenBounds())
                .AddTo(_disposables);
        }

        public Vector3 EnforceBounds(Vector3 currentPosition)
        {
            {
                var boundsAppliedToCurrentPosition = currentPosition;
                if (currentPosition.x > MaxBounds.x)
                {
                    boundsAppliedToCurrentPosition.x = MinBounds.x;
                }
                else if (currentPosition.x < MinBounds.x)
                {
                    boundsAppliedToCurrentPosition.x = MaxBounds.x;
                }

                if (currentPosition.y > MinBounds.y + .1f || currentPosition.y < MinBounds.y - .1f)
                {
                    boundsAppliedToCurrentPosition.y = MinBounds.y;
                }

                if (currentPosition.z > MaxBounds.z)
                {
                    boundsAppliedToCurrentPosition.z = MinBounds.z;
                }
                else if (currentPosition.z < MinBounds.z)
                {
                    boundsAppliedToCurrentPosition.z = MaxBounds.z;

                }
                return boundsAppliedToCurrentPosition;
            }
        }
        
        private void HandleGameOver(bool gameOver)
        {
            if (gameOver)
            {
                _disposables.Clear();
            }
        }
        
        private void SetScreenBounds()
        {
            _camDistance = Vector3.Distance(transform.position, _mainCamera.transform.position);
            _bottomCorner = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _camDistance));
            _topCorner = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _camDistance));

            MaxBounds = new Vector3(_topCorner.x, 1, _topCorner.z);
            MinBounds = new Vector3(_bottomCorner.x, 1, _bottomCorner.z);
        }
    }
}
