using MathConcepts;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.Features
{
    public class Elevator : Feature, IPowerable
    {
        public override string ObjectName => "Elevator";

#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private int _maxPower = 1;
        public int MaxPower => _maxPower;
        [SerializeField]
        private float _speed = 3f;
        public float Speed => _speed;
#pragma warning restore IDE0044 // Add readonly modifier

        public bool CanBePowered { get; private set; }
        public bool HasPower { get; private set; }

        private int _currentPower = 0;

        private IntVector2 _targetStop;
        public List<IntVector2> Stops { get; private set; } = new List<IntVector2>();

        private ElevatorDirection _direction = ElevatorDirection.Forward;

        public void Initialize(List<IntVector2> stops)
        {
            Stops = stops;

            _targetStop = IntVector2.Nearest(Position, Stops);

            CheckPoweredStatus();

            OnFeatureChanged.Raise(this);
        }

        private void Update()
        {
            if (HasPower)
            {
                if (Position == _targetStop)
                {
                    transform.position = Position;
                    _currentPower--;

                    CheckPoweredStatus();
                    SelectNextStop();
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, _targetStop, Time.deltaTime * _speed);
                }
            }
        }

        private void SelectNextStop()
        {
            var indexOf = Stops.IndexOf(_targetStop);

            if (_direction == ElevatorDirection.Forward)
            {
                if (indexOf < Stops.Count - 1)
                {
                    _targetStop = Stops.LoopedNext(indexOf);
                }
                else
                {
                    _direction = ElevatorDirection.Reverse;
                    SelectNextStop();
                }
            }
            else if (_direction == ElevatorDirection.Reverse)
            {
                if (indexOf > 0)
                {
                    _targetStop = Stops.LoopedPrevious(indexOf);
                }
                else
                {
                    _direction = ElevatorDirection.Forward;
                    SelectNextStop();
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
            Forward,
            Reverse
        }
    }
}