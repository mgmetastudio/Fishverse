using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class TileSelectHelper
    {

        #region Variables

        private static readonly Vector2[] fullCheckorder = new Vector2[] { new Vector2(-1, 0), new Vector2(0,-1), new Vector2(1,0), new Vector2(0,1),
            new Vector2(-1, -1), new Vector2(-1,1), new Vector2(1,-1), new Vector2(1,1) };
        private static readonly Vector2[] moveCheckorder = new Vector2[] { new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0), new Vector2(0, 1) };


        #endregion

        #region Public Methods

        public static void FindTiles(bool allowDiag, Transform transform, LayerMask layerMask, Vector2 startingPoint, Dictionary<Vector2, MoveableTile> locations, Dictionary<Vector2, int> movement, Dictionary<Vector2, List<Vector2>> paths, int movementLeft, List<Vector2> squaresUsed)
        {
            Vector2[] list = allowDiag ? fullCheckorder : moveCheckorder;
            Vector2 pos;
            MoveableTile tile;
            int remaingMovement;

            for (int i = 0; i < list.Length; i++)
            {
                pos = list[i] + startingPoint;
                if (!locations.ContainsKey(pos))
                {
                    locations.Add(pos, GetOffsetTile(transform, layerMask, pos));
                    movement.Add(pos, -100);
                }

                tile = locations[pos];
                if (tile != null)
                {
                    remaingMovement = movementLeft - tile.movementCost;
                    if (remaingMovement > movement[pos])
                    {
                        List<Vector2> localSquares = new List<Vector2>();
                        foreach (Vector2 square in squaresUsed)
                        {
                            localSquares.Add(square);
                        }
                        localSquares.Add(startingPoint);

                        movement[pos] = remaingMovement;
                        if (paths != null) paths[pos] = localSquares;
                        FindTiles(allowDiag, transform, layerMask, pos, locations, movement, paths, remaingMovement, localSquares);
                    }
                }
            }
        }

        public static MoveableTile GetOffsetTile(Transform transform, LayerMask layerMask, Vector2 relativePosiiton)
        {
            float checkDistance = 7.1f;
            Vector3 castFrom = new Vector3(transform.position.x + relativePosiiton.x, transform.position.y + 3f, transform.position.z + relativePosiiton.y);
            RaycastHit hit;
            if (Physics.Raycast(castFrom, -transform.up, out hit, checkDistance, layerMask))
            {
                MoveableTile result = hit.transform.gameObject.GetComponentInChildren<MoveableTile>();

                if (result == null)
                {
                    result = hit.transform.parent.gameObject.GetComponentInChildren<MoveableTile>();
                }
                return result;
            }

            return null;
        }



        #endregion

    }
}