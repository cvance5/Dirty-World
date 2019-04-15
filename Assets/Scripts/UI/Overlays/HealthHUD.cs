using System.Collections.Generic;
using UI.Components.UnityModifiers;
using UnityEngine;
using WorldObjects.Actors;

namespace UI.Overlays
{
    public class HealthHUD : UIOverlay
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly for Unity Serialization
        [SerializeField]
        private float _heightPerSegmentTile = 100f;

        [SerializeField]
        private GameObject _healthFillTemplate = null;
        [SerializeField]
        private Transform _healthFillContainer = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private Health _health = null;

        private RectTransform _containerTransform = null;

        private readonly Stack<FillBar> _activeSegmentFills = new Stack<FillBar>();
        private readonly Stack<FillBar> _inactiveSegmentFills = new Stack<FillBar>();

        private void Awake() => _containerTransform = GetComponent<RectTransform>();

        public void AssignHealth(Health health)
        {
            _health = health;
            _health.OnHealthChanged += OnHealthChanged;

            _containerTransform.sizeDelta = new Vector2(_containerTransform.sizeDelta.x, _health.MaxHealthSegments * _heightPerSegmentTile);

            for (var segmentNumber = _health.MaxHealthSegments - 1; segmentNumber >= 0; segmentNumber--)
            {
                CreateSegment(segmentNumber);
            }
        }

        private void CreateSegment(int segmentNumber)
        {
            var segmentPosition = new Vector2(0, -(segmentNumber * _heightPerSegmentTile));

            var segmentFill = Instantiate(_healthFillTemplate, _healthFillContainer).GetComponent<FillBar>();
            segmentFill.GetComponent<RectTransform>().anchoredPosition = segmentPosition;
            segmentFill.SetRange(0, _health.MaxHealthPerSegment);
            segmentFill.UpdateValue(_health.HealthForSegment(segmentNumber));
            _activeSegmentFills.Push(segmentFill);
        }

        private void ActivateSegment()
        {
            if (_inactiveSegmentFills.Count == 0) throw new System.InvalidOperationException($"No segments left to activate.");

            var activatedSegment = _inactiveSegmentFills.Pop();
            activatedSegment.SetActive(true);
            _activeSegmentFills.Push(activatedSegment);
        }

        private void DeactivateSegment()
        {
            if (_activeSegmentFills.Count == 0) throw new System.InvalidOperationException($"No segments left to deactivate.");

            var deactivatedSegment = _activeSegmentFills.Pop();
            deactivatedSegment.SetActive(false);
            _inactiveSegmentFills.Push(deactivatedSegment);
        }

        private void OnHealthChanged(int delta)
        {
            if (delta < 0)
            {
                do
                {
                    if (_activeSegmentFills.Count == 0)
                    {
                        delta = 0;
                    }
                    else
                    {
                        var nextSegment = _activeSegmentFills.Peek();
                        if (nextSegment.CurrentValue >= -delta)
                        {
                            nextSegment.ApplyDelta(delta);
                            delta = 0;
                        }
                        else
                        {
                            delta += Mathf.RoundToInt(nextSegment.CurrentValue);
                            nextSegment.SetFill(0);
                            DeactivateSegment();
                        }
                    }

                } while (delta < 0);
            }
            else
            {
                if (_activeSegmentFills.Count != 0)
                {
                    var currentSegment = _activeSegmentFills.Peek();

                    if (currentSegment != null && currentSegment.CurrentValue != 1)
                    {
                        var healingRequired = _health.MaxHealthPerSegment - currentSegment.CurrentValue;
                        if (healingRequired > delta)
                        {
                            currentSegment.ApplyDelta(delta);
                            delta = 0;
                        }
                        else
                        {
                            delta -= Mathf.RoundToInt(healingRequired);
                            currentSegment.SetFill(1);
                        }
                    }
                }

                while (delta > 0)
                {
                    if (_inactiveSegmentFills.Count == 0)
                    {
                        delta = 0;
                    }
                    else
                    {
                        ActivateSegment();
                        var nextSegment = _activeSegmentFills.Peek();
                        if (nextSegment.Range >= delta)
                        {
                            nextSegment.ApplyDelta(delta);
                            delta = 0;
                        }
                        else
                        {
                            delta -= Mathf.RoundToInt(nextSegment.Range);
                            nextSegment.SetFill(1);
                        }
                    }
                }
            }
        }
    }
}