using UnityEngine;

namespace WorldObjects.Actors.Player
{
    public class PlayerFlashlight : MonoBehaviour
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly for Unity Serialization
        [SerializeField]
        private Light _lightSource = null;

        [SerializeField]
        private float _lowIntensity = .5f;
        [SerializeField]
        private float _lowAngle = 5f;

        [SerializeField]
        private float _highIntensity = 1f;
        [SerializeField]
        private float _highAngle = 12.5f;
#pragma warning restore IDE0044 // Add readonly modifier

        private FlashlightMode _mode = FlashlightMode.Off;

        private PlayerHealth _healthSource = null;

        private void Awake()
        {
            _healthSource = GetComponent<PlayerHealth>();

            if (_healthSource == null)
            {
                throw new System.Exception($"No health source assigned to PlayerFlashlight.");
            }

            UpdateLightMode();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Flashlight"))
            {
                if (_mode == FlashlightMode.High)
                {
                    _mode = FlashlightMode.Off;
                    _healthSource.FillSegment();
                    _healthSource.FillSegment();
                }
                else
                {
                    if (_healthSource.TryEmptySegment())
                    {
                        if (_mode == FlashlightMode.Off)
                        {
                            _mode = FlashlightMode.Low;
                        }
                        else if (_mode == FlashlightMode.Low)
                        {
                            _mode = FlashlightMode.High;
                        }
                    }
                    else if (_mode == FlashlightMode.Low)
                    {
                        _mode = FlashlightMode.Off;
                        _healthSource.FillSegment();
                    }
                }

                UpdateLightMode();
            }

            if (_mode != FlashlightMode.Off)
            {
                UpdateLook();
            }
        }

        private void UpdateLightMode()
        {
            if (_mode == FlashlightMode.Off)
            {
                _lightSource.SetActive(false);
            }
            else _lightSource.SetActive(true);

            if (_mode == FlashlightMode.Off)
            {
                _lightSource.intensity = 0f;
                _lightSource.spotAngle = 0f;
            }
            else if (_mode == FlashlightMode.Low)
            {
                _lightSource.intensity = _lowIntensity;
                _lightSource.spotAngle = _lowAngle;
            }
            else if (_mode == FlashlightMode.High)
            {
                _lightSource.intensity = _highIntensity;
                _lightSource.spotAngle = _highAngle;
            }
        }

        private void UpdateLook()
        {
            var lookPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lookPosition.z = 0;
            _lightSource.transform.LookAt(lookPosition);
        }

        private enum FlashlightMode
        {
            Off,
            Low,
            High
        }
    }
}