using UniRx;
using UnityEngine;

namespace Misc
{
    public class BoundHandler : MonoBehaviour
    {
        public ReactiveProperty<Vector3> MaxBounds {get; private set;  }
        public ReactiveProperty<Vector3> MinBounds { get; private set; }
        
        private Camera _mainCamera;
        
        private void Awake()
        {
            MaxBounds = new ReactiveProperty<Vector3>(Vector3.zero);
            MinBounds = new ReactiveProperty<Vector3>(Vector3.zero);
            
            _mainCamera = Camera.main;
            SetScreenBounds();
        }

        public Vector3 EnforceBounds(Vector3 currentPosition)
        {
            {
                var boundsAppliedToCurrentPosition = currentPosition;
                
                if (currentPosition.x > MaxBounds.Value.x)
                {
                    boundsAppliedToCurrentPosition.x = MinBounds.Value.x;
                }
                else if (currentPosition.x < MinBounds.Value.x)
                {
                    boundsAppliedToCurrentPosition.x = MaxBounds.Value.x;
                }

                if (currentPosition.y > MinBounds.Value.y + .1f || currentPosition.y < MinBounds.Value.y - .1f)
                {
                    boundsAppliedToCurrentPosition.y = MinBounds.Value.y;
                }

                if (currentPosition.z > MaxBounds.Value.z)
                {
                    boundsAppliedToCurrentPosition.z = MinBounds.Value.z;
                }
                else if (currentPosition.z < MinBounds.Value.z)
                {
                    boundsAppliedToCurrentPosition.z = MaxBounds.Value.z;

                }
                return boundsAppliedToCurrentPosition;
            }
        }
        
        private void SetScreenBounds()
        {
            var camDistance = Vector3.Distance(transform.position, _mainCamera.transform.position);
            var bottomCorner = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
            var topCorner = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, camDistance));

            MaxBounds.Value = new Vector3(topCorner.x, 1, topCorner.z);
            MinBounds.Value = new Vector3(bottomCorner.x, 1, bottomCorner.z);
        }
    }
}
