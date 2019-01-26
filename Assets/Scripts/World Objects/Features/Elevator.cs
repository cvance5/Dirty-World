using UnityEngine;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.FeatureGeneration;

namespace WorldObjects.Features
{
    public class Elevator : Feature, IPowerable
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private int _maxPower = 1;
        [SerializeField]
        private float _speed = 3f;
#pragma warning restore IDE0044 // Add readonly modifier

        public override FeatureTypes Type => FeatureTypes.Elevator;
        public override string ObjectName => "Elevator";

        public bool CanBePowered { get; private set; }
        public bool HasPower { get; private set; }

        private int _currentPower = 0;

        private IntVector2 _highestStory;
        private IntVector2 _lowestStory;
        private ElevatorDirection _direction = ElevatorDirection.Up;

        public override void Initialize()
        {
            if (_containingSpace is Shaft)
            {
                var containingShaft = _containingSpace as Shaft;
                _lowestStory = containingShaft.BottomLeftCorner;
                _highestStory = new IntVector2(_lowestStory.X, containingShaft.TopRightCorner.Y - 2);
            }

            CheckPoweredStatus();
        }

        private void Update()
        {
            if (_currentPower == _maxPower)
            {
                if (_direction == ElevatorDirection.Up)
                {
                    transform.position += (Vector3.up * _speed * Time.deltaTime);
                    if (transform.position.y > _highestStory.Y)
                    {
                        transform.position = _highestStory;
                    }

                    if (transform.position == _highestStory)
                    {
                        _direction = ElevatorDirection.Down;
                    }
                }
                else
                {
                    transform.position += (Vector3.down * _speed * Time.deltaTime);
                    if (transform.position.y < _lowestStory.Y)
                    {
                        transform.position = _lowestStory;
                    }

                    if (transform.position == _lowestStory)
                    {
                        _direction = ElevatorDirection.Up;
                    }
                }
            }
        }

        public void AddPower()
        {
            _currentPower++;
            CheckPoweredStatus();
        }

        public void RemovePower()
        {
            _currentPower--;
            CheckPoweredStatus();
        }

        private void CheckPoweredStatus()
        {
            if (_currentPower > 0)
            {
                HasPower = true;
            }
            else HasPower = false;

            if (_currentPower == _maxPower)
            {
                CanBePowered = false;
            }
            else CanBePowered = true;
        }

        private enum ElevatorDirection
        {
            Up,
            Down
        }
    }
}