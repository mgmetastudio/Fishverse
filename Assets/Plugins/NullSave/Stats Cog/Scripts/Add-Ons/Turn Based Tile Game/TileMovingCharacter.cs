using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    [RequireComponent(typeof(StatsCog))]
    public class TileMovingCharacter : MonoBehaviour
    {

        #region Variables

        public LayerMask movementLayer;
        public LayerMask collisionMask;

        public string movementStat = "Speed";
        public bool canMoveDiagonal = true;
        public float moveSpeed = 2;

        public string animMovement = "Speed";

        public UnityEvent onMovementStart, onMovementEnd;

        private readonly Vector2[] fullCheckorder = new Vector2[] { new Vector2(-1, 0), new Vector2(0,-1), new Vector2(1,0), new Vector2(0,1),
            new Vector2(-1, -1), new Vector2(-1,1), new Vector2(1,-1), new Vector2(1,1) };
        private readonly Vector2[] moveCheckorder = new Vector2[] { new Vector2(-1, 0), new Vector2(0,-1), new Vector2(1,0), new Vector2(0,1) };

        private List<Vector2> curPath;
        private int pathIndex;
        private float pathTween;

        private bool showMovement;

        private Vector3 startPos;
        private int endMovement;

        internal Dictionary<Vector2, MoveableTile> locations = new Dictionary<Vector2, MoveableTile>();
        internal Dictionary<Vector2, int> movements = new Dictionary<Vector2, int>();
        internal Dictionary<Vector2, List<Vector2>> paths = new Dictionary<Vector2, List<Vector2>>();

        #endregion

        #region Properties

        public Animator Animator { get; private set; }

        public bool IsMoving { get; private set; }

        public StatValue Movement { get; private set; }

        public bool ShowMovement
        {
            get { return showMovement; }
            set
            {
                if (value == showMovement) return;

                showMovement = value;
                if (showMovement)
                {
                    CalculateMovement();

                    foreach (var movement in movements)
                    {
                        if (movement.Value > -1 && locations[movement.Key] != null)
                        {
                            locations[movement.Key].ShowMoveMarker = true;
                        }
                    }
                }
                else
                {
                    foreach (var movement in movements)
                    {
                        if (movement.Value > -1 && locations[movement.Key] != null)
                        {
                            locations[movement.Key].ShowMoveMarker = false;
                        }
                    }
                }
            }
        }

        public StatsCog StatSource { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Animator = GetComponentInChildren<Animator>();
            StatSource = GetComponent<StatsCog>();
            Movement = StatSource.FindStat(movementStat);
        }

        #endregion

        #region Public Methods

        public void CalculateMovement()
        {
            int availableMovement = (int)Movement.CurrentValue;

            movements.Clear();
            locations.Clear();
            paths.Clear();
            List<Vector2> squaresUsed = new List<Vector2>();

            TileSelectHelper.FindTiles(canMoveDiagonal, transform, movementLayer, Vector2.zero, locations, movements, paths, availableMovement, squaresUsed);
        }

        public bool MoveTo(RaycastHit pos)
        {
            float checkDistance = 7.1f;
            Vector3 castFrom = new Vector3(pos.transform.position.x, 3, pos.transform.position.z);
            RaycastHit hit;

            if (Physics.Raycast(castFrom, -transform.up, out hit, checkDistance, movementLayer))
            {
                MoveableTile tile = hit.transform.gameObject.GetComponentInChildren<MoveableTile>();
                if(tile == null) tile = hit.transform.gameObject.GetComponentInParent<MoveableTile>();
                if (tile != null)
                {
                    Vector2 offset = new Vector2(Mathf.Floor(hit.transform.position.x - transform.position.x), Mathf.Floor(hit.transform.position.z - transform.position.z));

                    if (offset == Vector2.zero) return false;

                    if (paths.ContainsKey(offset))
                    {
                        if (movements[offset] < 0)
                        {
                            return false;
                        }

                        PerformMove(offset);
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Private Methods

        private IEnumerator MoveToTarget()
        {
            onMovementStart?.Invoke();
            Vector3 lastPos = transform.position;

            pathIndex = 1;
            while (pathIndex < curPath.Count)
            {
                Vector3 nextPoint = new Vector3(startPos.x + curPath[pathIndex].x, startPos.y, startPos.z + curPath[pathIndex].y);
                transform.LookAt(nextPoint);

                while (pathTween < 1)
                {
                    pathTween = Mathf.Clamp(pathTween + Time.deltaTime * moveSpeed, 0, 1);
                    if (pathIndex == 1 && Animator != null) Animator.SetFloat(animMovement, pathTween);
                    transform.position = Vector3.Lerp(lastPos, nextPoint, pathTween);
                    yield return new WaitForEndOfFrame();
                }

                pathIndex += 1;
                pathTween = 0;
                lastPos = transform.position;
            }

            Movement.SetValue(endMovement);
            IsMoving = false;
            if (Animator != null) Animator.SetFloat(animMovement, 0);
            onMovementEnd?.Invoke();
        }

        internal void PerformMove(Vector2 key)
        {
            IsMoving = true;
            curPath = paths[key];
            curPath.Add(key);
            pathIndex = 0;
            pathTween = 0;
            startPos = transform.position;
            endMovement = movements[key];

            StartCoroutine(MoveToTarget());
        }

        #endregion

    }
}