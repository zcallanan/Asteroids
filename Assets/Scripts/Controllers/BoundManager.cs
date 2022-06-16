using UnityEngine;

namespace Controllers
{
    public class BoundManager : MonoBehaviour
    {
        public static BoundManager sharedInstance;
        public float MinX { get; private set; }
        public float MaxX { get; private set; }
        public float MinZ { get; private set; }
        public float MaxZ { get; private set; }
        private static float _boundY;
        private static float _camDistance;
        private static Vector3 _bottomCorner;
        private static Vector3 _topCorner;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            sharedInstance = this;
            SetScreenBounds();
            _boundY = 1.0f;
            GameManager.sharedInstance.OnScreenSizeChange += SetScreenBounds;
        }

        private void SetScreenBounds()
        {
            _camDistance = Vector3.Distance(transform.position, _mainCamera.transform.position);
            _bottomCorner = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _camDistance));
            _topCorner = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _camDistance));
            MinX = _bottomCorner.x;
            MaxX = _topCorner.x;
            MinZ = _bottomCorner.z;
            MaxZ = _topCorner.z;
        }

        public Vector3 EnforceBounds(Vector3 currentPosition)
        {
            {
                var boundsAppliedToCurrentPosition = currentPosition;
                if (currentPosition.x > MaxX)
                {
                    boundsAppliedToCurrentPosition.x = MinX;
                }
                else if (currentPosition.x < MinX)
                {
                    boundsAppliedToCurrentPosition.x = MaxX;
                }

                if (currentPosition.y > _boundY + .1f || currentPosition.y < _boundY - .1f)
                {
                    boundsAppliedToCurrentPosition.y = _boundY;
                }

                if (currentPosition.z > MaxZ)
                {
                    boundsAppliedToCurrentPosition.z = MinZ;
                }
                else if (currentPosition.z < MinZ)
                {
                    boundsAppliedToCurrentPosition.z = MaxZ;

                }
                return boundsAppliedToCurrentPosition;
            }
        }
    }
}
