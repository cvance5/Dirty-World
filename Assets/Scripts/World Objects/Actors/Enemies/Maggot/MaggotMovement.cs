using System.Collections;
using UnityEngine;
using WorldObjects;

namespace WorldObjects.Actors.Enemies.Maggot
{
    public class MaggotMovement : MonoBehaviour
    {
#pragma warning disable IDE0044 // Add readonly modifier, Unity serialization requires it not be readonly
        [SerializeField]
        private float _moveDelay = 0;
        [SerializeField]
        private float _moveDistance = 0;
        [SerializeField]
        private float _moveAnimationSpeed = 0;

        [SerializeField]
        private float _turnAnimationSpeed = 0;

        [SerializeField]
        private GameObject _frontHalf = null;
        [SerializeField]
        private GameObject _backHalf = null;

        [SerializeField]
        private ActorFeet _feet = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private BoxCollider2D _collider;

        private Coroutine _moveDelayCoroutine;

        private ActorFeetState _state;

        private bool _shouldTurn = false;

        private void Awake()
        {
            if (_feet.IsColliding)
            {
                _state = ActorFeetState.Grounded;
            }

            _feet.OnFootTouch += OnLand;
            _feet.OnFootLeave += OnAirborne;

            if (_state == ActorFeetState.Grounded)
            {
                _moveDelayCoroutine = StartCoroutine(Move());
            }

            _collider = GetComponent<BoxCollider2D>();

            var footCollider = _feet.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(_collider, footCollider, true);
        }

        private IEnumerator Move()
        {
            var wfDelay = new WaitForSeconds(_moveDelay);

            while (true)
            {
                yield return wfDelay;
                yield return StartCoroutine(MoveStep(_frontHalf, _backHalf));
                yield return StartCoroutine(MoveStep(_backHalf, _frontHalf));

                if (_shouldTurn)
                {
                    yield return StartCoroutine(Turn());
                }
            }
        }

        private IEnumerator MoveStep(GameObject forwardBodyPart, GameObject backwardBodyPart)
        {
            var bumpedSomething = false;

            void onWorldObjectBumped(WorldObject bumpedObject)
            {
                bumpedSomething = true;
                _shouldTurn = true;
            }

            var forwardBumpDetector = forwardBodyPart.GetComponent<ActorBumpDetector>();
            forwardBumpDetector.OnWorldObjectBumped += onWorldObjectBumped;

            var pivotPointStartLocation = transform.position;
            var pivotPointEndPosition = pivotPointStartLocation + ((_moveDistance / 2) * transform.right);

            var fowardBodyPartStartLocation = forwardBodyPart.transform.localPosition;
            var forwardBodyPartEndLocation = fowardBodyPartStartLocation + ((_moveDistance / 2) * transform.right);

            var backwardBodyPartStartLocation = backwardBodyPart.transform.localPosition;
            var backwardBodyPartEndLocation = backwardBodyPartStartLocation - ((_moveDistance / 2) * transform.right);

            var timePassed = 0f;

            while (timePassed < _moveAnimationSpeed && !bumpedSomething)
            {
                timePassed += Time.deltaTime;
                var smoothedTime = Mathf.SmoothStep(0, 1, timePassed / _moveAnimationSpeed);

                transform.position = Vector2.Lerp(pivotPointStartLocation, pivotPointEndPosition, smoothedTime);
                forwardBodyPart.transform.localPosition = Vector2.Lerp(fowardBodyPartStartLocation, forwardBodyPartEndLocation, smoothedTime);
                backwardBodyPart.transform.localPosition = Vector2.Lerp(backwardBodyPartStartLocation, backwardBodyPartEndLocation, smoothedTime);

                _collider.size = new Vector2(1 + Mathf.Abs(forwardBodyPart.transform.localPosition.x - backwardBodyPart.transform.localPosition.x),
                                             1 + Mathf.Abs(forwardBodyPart.transform.localPosition.y - backwardBodyPart.transform.localPosition.y));

                yield return null;
            }

            forwardBumpDetector.OnWorldObjectBumped -= onWorldObjectBumped;
        }

        private IEnumerator Turn()
        {
            QuickReset();

            var timePassed = 0f;

            var startRotation = transform.rotation.eulerAngles;
            var endRotation = startRotation + (Vector3.up * 180);

            while (timePassed < _turnAnimationSpeed)
            {
                timePassed += Time.deltaTime;
                var smoothedTime = Mathf.SmoothStep(0, 1, timePassed / _turnAnimationSpeed);
                var vectorRotation = Vector3.Lerp(startRotation, endRotation, smoothedTime);

                transform.rotation = Quaternion.Euler(vectorRotation);

                yield return null;
            }

            _shouldTurn = false;
        }

        private void OnLand()
        {
            _state = ActorFeetState.Grounded;

            if (_moveDelayCoroutine == null)
            {
                _moveDelayCoroutine = StartCoroutine(Move());

                var deltaToFaceLeft = Vector2.Angle(transform.right, Vector2.left);
                var deltaToFaceRight = Vector2.Angle(transform.right, Vector2.right);

                if (deltaToFaceLeft >= deltaToFaceRight)
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else transform.rotation = Quaternion.identity;

                QuickReset();
            }
        }

        private void OnAirborne()
        {
            _state = ActorFeetState.Airborne;

            if (_moveDelayCoroutine != null)
            {
                StopCoroutine(_moveDelayCoroutine);
                _moveDelayCoroutine = null;

                QuickReset();
            }
        }

        private void QuickReset()
        {
            _frontHalf.transform.localPosition = Vector2.zero;
            _backHalf.transform.localPosition = Vector2.zero;

            _collider.size = Vector2.one;
        }
    }
}