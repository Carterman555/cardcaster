#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.UI.Image;

// matches the points of the polygon colliders for the room and the camera confiner
public class RoomColliderMatcher {

    private enum Corner {
        OuterTopLeft, OuterTopRight, OuterBottomLeft, OuterBottomRight,
        InnerTopLeft, InnerTopRight, InnerBottomLeft, InnerBottomRight
    }

    private PolygonCollider2D roomCollider;
    private PolygonCollider2D camConfinerCollider;

    private Tilemap topWallsTilemap;
    private Tilemap botWallsTilemap;

    public RoomColliderMatcher(PolygonCollider2D roomCollider, PolygonCollider2D camConfinerCollider, Tilemap topWallsTilemap, Tilemap botWallsTilemap) {
        this.roomCollider = roomCollider;
        this.camConfinerCollider = camConfinerCollider;
        this.topWallsTilemap = topWallsTilemap;
        this.botWallsTilemap = botWallsTilemap;
    }

    #region Setup Room Collider
    // This method sets up the points of the room polygon collider to create a border around the room following the
    // walls of the tilemap
    // 1) Find the starting point to place and place a point there.
    // 2) From this starting point point travel along the tilemap walls, place a point at any corner
    public void SetupRoomCollider() {
        Vector2 startingColliderPosition = TopLeftTileOuterPosition();
        roomCollider.points = new Vector2[] { startingColliderPosition };

        Vector3Int startingTilePosition = TopLeftTilePosition();
        AddOutlinePoints(startingTilePosition);
    }

    private void AddOutlinePoints(Vector3Int startingPosition) {

        Vector2 traceDirection = Vector2.right;

        Vector3Int currentTilePosition = startingPosition;
        currentTilePosition += new Vector3Int((int)traceDirection.x, (int)traceDirection.y);

        int safeguardCounter = 0;

        // loop until it loops back to the original point
        while (currentTilePosition != startingPosition) {

            safeguardCounter++;
            if (safeguardCounter > 1000) {
                Debug.LogError("Exceeded maximum iterations!");
                return;
            }

            Vector2 previousDirection = traceDirection;

            bool directionChanged = UpdateTraceDirection(ref traceDirection, currentTilePosition);

            if (directionChanged) {
                Vector2 pointPosition = (Vector2)(Vector3)currentTilePosition + GetTileOutsidePosition(previousDirection, traceDirection);
                AddPoint(roomCollider, pointPosition);
            }

            currentTilePosition += new Vector3Int((int)traceDirection.x, (int)traceDirection.y);
        }
    }

    // when adding outline points, this method guides the direction to move around the tilemaps in a clock wise
    // return whether it turned
    private bool UpdateTraceDirection(ref Vector2 traceDirection, Vector3Int currentTilePosition) {

        Vector3Int aboveTilePos = currentTilePosition + new Vector3Int(0, 1);
        Vector3Int belowTilePos = currentTilePosition + new Vector3Int(0, -1);
        Vector3Int rightTilePos = currentTilePosition + new Vector3Int(1, 0);
        Vector3Int leftTilePos = currentTilePosition + new Vector3Int(-1, 0);

        bool tileAbove = TileAtPosition(aboveTilePos);
        bool tileBelow = TileAtPosition(belowTilePos);
        bool tileToRight = TileAtPosition(rightTilePos);
        bool tileToLeft = TileAtPosition(leftTilePos);

        // When tracing UP - we're moving along the LEFT edge of the shape
        // 1. If there's a tile to the left, turn left to follow the boundary
        // 2. If there's no tile above, turn right as we've reached a corner
        if (traceDirection == Vector2.up) {
            if (tileToLeft) {
                traceDirection = Vector2.left;
                return true;
            }
            else if (!tileAbove) {
                traceDirection = Vector2.right;
                return true;
            }
        }

        // When tracing DOWN - we're moving along the RIGHT edge of the shape
        // 1. If there's a tile to the right, turn right to follow the boundary
        // 2. If there's no tile below, turn left as we've reached a corner
        else if (traceDirection == Vector2.down) {
            if (tileToRight) {
                traceDirection = Vector2.right;
                return true;
            }
            else if (!tileBelow) {
                traceDirection = Vector2.left;
                return true;
            }
        }

        // When tracing RIGHT - we're moving along the TOP edge of the shape
        // 1. If there's a tile above, turn up to follow the boundary
        // 2. If there's no tile to the right, turn down as we've reached a corner
        else if (traceDirection == Vector2.right) {
            if (tileAbove) {
                traceDirection = Vector2.up;
                return true;
            }
            else if (!tileToRight) {
                traceDirection = Vector2.down;
                return true;
            }
        }

        // When tracing LEFT - we're moving along the BOTTOM edge of the shape
        // 1. If there's a tile below, turn down to follow the boundary
        // 2. If there's no tile to the left, turn up as we've reached a corner
        else if (traceDirection == Vector2.left) {
            if (tileBelow) {
                traceDirection = Vector2.down;
                return true;
            }
            else if (!tileToLeft) {
                traceDirection = Vector2.up;
                return true;
            }
        }

        return false;
    }

    private Vector2 GetTileOutsidePosition(Vector2 previousDirection, Vector2 newDirection) {

        // up > right = (0, 1)
        if (previousDirection == Vector2.up && newDirection == Vector2.right) {
            return new Vector2(0, 1);
        }
        // up > left = (0, 0)
        if (previousDirection == Vector2.up && newDirection == Vector2.left) {
            return new Vector2(0, 0);
        }
        // down > right = (1, 1)
        if (previousDirection == Vector2.down && newDirection == Vector2.right) {
            return new Vector2(1, 1);
        }
        // down > left = (1, 0)
        if (previousDirection == Vector2.down && newDirection == Vector2.left) {
            return new Vector2(1, 0);
        }
        // right > down = (1, 1)
        if (previousDirection == Vector2.right && newDirection == Vector2.down) {
            return new Vector2(1, 1);
        }
        // right > up = (0, 1)
        if (previousDirection == Vector2.right && newDirection == Vector2.up) {
            return new Vector2(0, 1);
        }
        // left > down = (1, 0)
        if (previousDirection == Vector2.left && newDirection == Vector2.down) {
            return new Vector2(1, 0);
        }
        // left > up = (0, 0)
        if (previousDirection == Vector2.left && newDirection == Vector2.up) {
            return new Vector2(0, 0);
        }

        Debug.LogError($"Unhandled direction change: {previousDirection} to {newDirection}");
        return Vector2.zero;
    }

    private bool TileAtPosition(Vector3Int position) {
        bool tileInTopWalls = topWallsTilemap.GetTile(position) != null;
        bool tileInBotWalls = botWallsTilemap.GetTile(position) != null;
        return tileInTopWalls || tileInBotWalls;
    }

    #endregion


    #region Setup Camera Confiner Collider

    public void SetupCamConfinerCollider() {

        List<Vector2> camConfinerPoints = new List<Vector2>(roomCollider.points);

        float minHeight = Camera.main.orthographicSize * 2f;
        float minWidth = minHeight * Camera.main.aspect;

        camConfinerPoints = EnsureMinimumDimensions(camConfinerPoints, minWidth, minHeight);
        camConfinerPoints = AddPadding(camConfinerPoints);
        // TODO - Remove concaves and intersections

        camConfinerCollider.points = camConfinerPoints.ToArray();
    }

    /// <summary>
    /// Go through each inner point.
    ///     Find the 2 distances (vertical and horizontal) between itself and the points across the room
    ///     If the horizontal distance is less than the min width
    ///         expand points on both sides (each halfway) (could be 2 or 3 points)
    ///     If the vertical distance is less than the min height
    ///         expand points on both sides (each halfway) (could be 2 or 3 points)
    /// 
    /// </summary>
    private List<Vector2> EnsureMinimumDimensions(List<Vector2> points, float minWidth, float minHeight) {
        List<Vector2> newPoints = new List<Vector2>(points);

        for (int currentIndex = 0; currentIndex < newPoints.Count; currentIndex++) {

            // Use modulo to wrap around
            int prevIndex = (currentIndex - 1 + newPoints.Count) % newPoints.Count;
            int nextIndex = (currentIndex + 1) % newPoints.Count;

            Vector2 previousPoint = newPoints[prevIndex];
            Vector2 currentPoint = newPoints[currentIndex];
            Vector2 nextPoint = newPoints[nextIndex];

            Corner currentCorner = GetPointCorner(previousPoint, currentPoint, nextPoint);

            if (currentCorner == Corner.InnerTopLeft) {

                Debug.Log($"Expanding With Inner Top Left Corner Point: {currentPoint}");

                int[] horizontalAcrossPointIndexes = null;
                float minHorizontalDistance = float.PositiveInfinity;

                int[] verticalAcrossPointIndexes = null;
                float minVerticalDistance = float.PositiveInfinity;

                for (int searchIndex1 = 0; searchIndex1 < newPoints.Count; searchIndex1++) {
                    int searchIndex2 = (searchIndex1 + 1) % newPoints.Count;

                    Vector2 point1 = newPoints[searchIndex1];
                    Vector2 point2 = newPoints[searchIndex2];

                    Debug.Log($"  Search points {point1} and {point2}");

                    bool pointsMakeVerticalLine = point1.x == point2.x;
                    bool pointsMakeHorizontalLine = point1.y == point2.y;
                    if (pointsMakeVerticalLine) {

                        if (currentPoint.x >= point1.x) {
                            continue;
                        }

                        float minY = Mathf.Min(point1.y, point2.y);
                        float maxY = Mathf.Max(point1.y, point2.y);
                        bool betweenYPositions = minY <= currentPoint.y && maxY >= currentPoint.y;
                        if (betweenYPositions) {
                            Debug.Log($"    Points are across horizontally");

                            float horizontalDistance = Mathf.Abs(point1.x - currentPoint.x);
                            if (horizontalDistance < minHorizontalDistance) {
                                horizontalAcrossPointIndexes = new int[] { searchIndex1, searchIndex2 };
                                minHorizontalDistance = horizontalDistance;
                            }
                        }
                    }
                    else if (pointsMakeHorizontalLine) {

                        if (currentPoint.y <= point1.y) {
                            continue;
                        }

                        float minX = Mathf.Min(point1.x, point2.x);
                        float maxX = Mathf.Max(point1.x, point2.x);
                        bool betweenXPositions = minX <= currentPoint.x && maxX >= currentPoint.x;
                        if (betweenXPositions) {
                            Debug.Log($"    Points are across vertically");

                            float verticalDistance = Mathf.Abs(point1.y - currentPoint.y);
                            if (verticalDistance < minVerticalDistance) {
                                verticalAcrossPointIndexes = new int[] { searchIndex1, searchIndex2 };
                                minVerticalDistance = verticalDistance;
                            }
                        }
                    }
                    else {
                        Debug.LogError("Points don't make vertical or horizontal line!");
                        continue;
                    }
                }

                if (minVerticalDistance == float.PositiveInfinity) {
                    Debug.LogError("Did not find points that were across vertically!");
                    return newPoints;
                }

                if (minHorizontalDistance == float.PositiveInfinity) {
                    Debug.LogError("Did not find points that were across horizontally!");
                    return newPoints;
                }

                if (minHorizontalDistance < minWidth) {
                    float totalExpansion = minWidth - minHorizontalDistance;
                    float oneSideExpansion = Mathf.CeilToInt(totalExpansion / 2f);

                    Vector2 original = currentPoint;
                    currentPoint.x -= oneSideExpansion;
                    newPoints[currentIndex] = currentPoint;

                    nextPoint.x -= oneSideExpansion;
                    newPoints[nextIndex] = nextPoint;

                    Vector2 point1 = newPoints[horizontalAcrossPointIndexes[0]];
                    Vector2 original1 = point1;
                    point1.x += oneSideExpansion;
                    newPoints[horizontalAcrossPointIndexes[0]] = point1;

                    Vector2 point2 = newPoints[horizontalAcrossPointIndexes[1]];
                    Vector2 original2 = point2;
                    point2.x += oneSideExpansion;
                    newPoints[horizontalAcrossPointIndexes[1]] = point2;

                    Debug.Log($"  Expanding horizontally:\n" +
                              $"    Corner: {original} > {currentPoint}\n" +
                              $"    Opposite 1: {original1} > {point1}\n" +
                              $"    Opposite 2: {original2} > {point2}");
                }

                if (minVerticalDistance < minHeight) {
                    float totalExpansion = minHeight - minVerticalDistance;
                    float oneSideExpansion = Mathf.CeilToInt(totalExpansion / 2f);

                    Vector2 original = currentPoint;
                    currentPoint.y += oneSideExpansion;
                    newPoints[currentIndex] = currentPoint;

                    previousPoint.y += oneSideExpansion;
                    newPoints[prevIndex] = previousPoint;

                    Vector2 point1 = newPoints[verticalAcrossPointIndexes[0]];
                    Vector2 original1 = point1;
                    point1.y -= oneSideExpansion;
                    newPoints[verticalAcrossPointIndexes[0]] = point1;

                    Vector2 point2 = newPoints[verticalAcrossPointIndexes[1]];
                    Vector2 original2 = point2;
                    point2.y -= oneSideExpansion;
                    newPoints[verticalAcrossPointIndexes[1]] = point2;

                    Debug.Log($"  Expanding vertically:\n" +
                              $"    Corner: {original} > {currentPoint}\n" +
                              $"    Opposite 1: {original1} > {point1}\n" +
                              $"    Opposite 2: {original2} > {point2}");
                }
            }
            else if (currentCorner == Corner.InnerTopRight) {

                Debug.Log($"Expanding With Inner Top Right Corner Point: {currentPoint}");

                int[] horizontalAcrossPointIndexes = null;
                float minHorizontalDistance = float.PositiveInfinity;

                int[] verticalAcrossPointIndexes = null;
                float minVerticalDistance = float.PositiveInfinity;

                for (int searchIndex1 = 0; searchIndex1 < newPoints.Count; searchIndex1++) {
                    int searchIndex2 = (searchIndex1 + 1) % newPoints.Count;

                    Vector2 point1 = newPoints[searchIndex1];
                    Vector2 point2 = newPoints[searchIndex2];

                    Debug.Log($"  Search points {point1} and {point2}");

                    bool pointsMakeVerticalLine = point1.x == point2.x;
                    bool pointsMakeHorizontalLine = point1.y == point2.y;
                    if (pointsMakeVerticalLine) {

                        if (currentPoint.x <= point1.x) {
                            continue;
                        }

                        float minY = Mathf.Min(point1.y, point2.y);
                        float maxY = Mathf.Max(point1.y, point2.y);
                        bool betweenYPositions = minY <= currentPoint.y && maxY >= currentPoint.y;
                        if (betweenYPositions) {
                            Debug.Log($"    Points are across horizontally");

                            float horizontalDistance = Mathf.Abs(point1.x - currentPoint.x);
                            if (horizontalDistance < minHorizontalDistance) {
                                horizontalAcrossPointIndexes = new int[] { searchIndex1, searchIndex2 };
                                minHorizontalDistance = horizontalDistance;
                            }
                        }
                    }
                    else if (pointsMakeHorizontalLine) {

                        if (currentPoint.y <= point1.y) {
                            continue;
                        }

                        float minX = Mathf.Min(point1.x, point2.x);
                        float maxX = Mathf.Max(point1.x, point2.x);
                        bool betweenXPositions = minX <= currentPoint.x && maxX >= currentPoint.x;
                        if (betweenXPositions) {
                            Debug.Log($"    Points are across vertically");

                            float verticalDistance = Mathf.Abs(point1.y - currentPoint.y);
                            if (verticalDistance < minVerticalDistance) {
                                verticalAcrossPointIndexes = new int[] { searchIndex1, searchIndex2 };
                                minVerticalDistance = verticalDistance;
                            }
                        }
                    }
                    else {
                        Debug.LogError("Points don't make vertical or horizontal line!");
                        continue;
                    }
                }

                if (minVerticalDistance == float.PositiveInfinity) {
                    Debug.LogError("Did not find points that were across vertically!");
                    return newPoints;
                }

                if (minHorizontalDistance == float.PositiveInfinity) {
                    Debug.LogError("Did not find points that were across horizontally!");
                    return newPoints;
                }

                if (minHorizontalDistance < minWidth) {
                    float totalExpansion = minWidth - minHorizontalDistance;
                    float oneSideExpansion = Mathf.CeilToInt(totalExpansion / 2f);

                    Vector2 original = currentPoint;
                    currentPoint.x += oneSideExpansion;
                    newPoints[currentIndex] = currentPoint;

                    previousPoint.x += oneSideExpansion;
                    newPoints[prevIndex] = previousPoint;

                    Vector2 point1 = newPoints[horizontalAcrossPointIndexes[0]];
                    Vector2 original1 = point1;
                    point1.x -= oneSideExpansion;
                    newPoints[horizontalAcrossPointIndexes[0]] = point1;

                    Vector2 point2 = newPoints[horizontalAcrossPointIndexes[1]];
                    Vector2 original2 = point2;
                    point2.x -= oneSideExpansion;
                    newPoints[horizontalAcrossPointIndexes[1]] = point2;

                    Debug.Log($"  Expanding horizontally:\n" +
                              $"    Corner: {original} > {currentPoint}\n" +
                              $"    Opposite 1: {original1} > {point1}\n" +
                              $"    Opposite 2: {original2} > {point2}");
                }

                if (minVerticalDistance < minHeight) {
                    float totalExpansion = minHeight - minVerticalDistance;
                    float oneSideExpansion = Mathf.CeilToInt(totalExpansion / 2f);

                    Vector2 original = currentPoint;
                    currentPoint.y += oneSideExpansion;
                    newPoints[currentIndex] = currentPoint;

                    nextPoint.y += oneSideExpansion;
                    newPoints[nextIndex] = nextPoint;

                    Vector2 point1 = newPoints[verticalAcrossPointIndexes[0]];
                    Vector2 original1 = point1;
                    point1.y -= oneSideExpansion;
                    newPoints[verticalAcrossPointIndexes[0]] = point1;

                    Vector2 point2 = newPoints[verticalAcrossPointIndexes[1]];
                    Vector2 original2 = point2;
                    point2.y -= oneSideExpansion;
                    newPoints[verticalAcrossPointIndexes[1]] = point2;

                    Debug.Log($"  Expanding vertically:\n" +
                              $"    Corner: {original} > {currentPoint}\n" +
                              $"    Opposite 1: {original1} > {point1}\n" +
                              $"    Opposite 2: {original2} > {point2}");
                }
            }
            else if (currentCorner == Corner.InnerBottomLeft) {

                Debug.Log($"Expanding With Inner Bottom Left Corner Point: {currentPoint}");

                int[] horizontalAcrossPointIndexes = null;
                float minHorizontalDistance = float.PositiveInfinity;

                int[] verticalAcrossPointIndexes = null;
                float minVerticalDistance = float.PositiveInfinity;

                for (int searchIndex1 = 0; searchIndex1 < newPoints.Count; searchIndex1++) {
                    int searchIndex2 = (searchIndex1 + 1) % newPoints.Count;

                    Vector2 point1 = newPoints[searchIndex1];
                    Vector2 point2 = newPoints[searchIndex2];

                    Debug.Log($"  Search points {point1} and {point2}");

                    bool pointsMakeVerticalLine = point1.x == point2.x;
                    bool pointsMakeHorizontalLine = point1.y == point2.y;
                    if (pointsMakeVerticalLine) {

                        if (currentPoint.x >= point1.x) {
                            continue;
                        }

                        float minY = Mathf.Min(point1.y, point2.y);
                        float maxY = Mathf.Max(point1.y, point2.y);
                        bool betweenYPositions = minY <= currentPoint.y && maxY >= currentPoint.y;
                        if (betweenYPositions) {
                            Debug.Log($"    Points are across horizontally");

                            float horizontalDistance = Mathf.Abs(point1.x - currentPoint.x);
                            if (horizontalDistance < minHorizontalDistance) {
                                horizontalAcrossPointIndexes = new int[] { searchIndex1, searchIndex2 };
                                minHorizontalDistance = horizontalDistance;
                            }
                        }
                    }
                    else if (pointsMakeHorizontalLine) {

                        if (currentPoint.y >= point1.y) {
                            continue;
                        }

                        float minX = Mathf.Min(point1.x, point2.x);
                        float maxX = Mathf.Max(point1.x, point2.x);
                        bool betweenXPositions = minX <= currentPoint.x && maxX >= currentPoint.x;
                        if (betweenXPositions) {
                            Debug.Log($"    Points are across vertically");

                            float verticalDistance = Mathf.Abs(point1.y - currentPoint.y);
                            if (verticalDistance < minVerticalDistance) {
                                verticalAcrossPointIndexes = new int[] { searchIndex1, searchIndex2 };
                                minVerticalDistance = verticalDistance;
                            }
                        }
                    }
                    else {
                        Debug.LogError("Points don't make vertical or horizontal line!");
                        continue;
                    }
                }

                if (minVerticalDistance == float.PositiveInfinity) {
                    Debug.LogError("Did not find points that were across vertically!");
                    return newPoints;
                }

                if (minHorizontalDistance == float.PositiveInfinity) {
                    Debug.LogError("Did not find points that were across horizontally!");
                    return newPoints;
                }

                if (minHorizontalDistance < minWidth) {
                    float totalExpansion = minWidth - minHorizontalDistance;
                    float oneSideExpansion = Mathf.CeilToInt(totalExpansion / 2f);

                    Vector2 original = currentPoint;
                    currentPoint.x -= oneSideExpansion;
                    newPoints[currentIndex] = currentPoint;

                    previousPoint.x -= oneSideExpansion;
                    newPoints[prevIndex] = previousPoint;

                    Vector2 point1 = newPoints[horizontalAcrossPointIndexes[0]];
                    Vector2 original1 = point1;
                    point1.x += oneSideExpansion;
                    newPoints[horizontalAcrossPointIndexes[0]] = point1;

                    Vector2 point2 = newPoints[horizontalAcrossPointIndexes[1]];
                    Vector2 original2 = point2;
                    point2.x += oneSideExpansion;
                    newPoints[horizontalAcrossPointIndexes[1]] = point2;

                    Debug.Log($"  Expanding horizontally:\n" +
                              $"    Corner: {original} > {currentPoint}\n" +
                              $"    Opposite 1: {original1} > {point1}\n" +
                              $"    Opposite 2: {original2} > {point2}");
                }

                if (minVerticalDistance < minHeight) {
                    float totalExpansion = minHeight - minVerticalDistance;
                    float oneSideExpansion = Mathf.CeilToInt(totalExpansion / 2f);

                    Vector2 original = currentPoint;
                    currentPoint.y -= oneSideExpansion;
                    newPoints[currentIndex] = currentPoint;

                    nextPoint.y -= oneSideExpansion;
                    newPoints[nextIndex] = nextPoint;

                    Vector2 point1 = newPoints[verticalAcrossPointIndexes[0]];
                    Vector2 original1 = point1;
                    point1.y += oneSideExpansion;
                    newPoints[verticalAcrossPointIndexes[0]] = point1;

                    Vector2 point2 = newPoints[verticalAcrossPointIndexes[1]];
                    Vector2 original2 = point2;
                    point2.y += oneSideExpansion;
                    newPoints[verticalAcrossPointIndexes[1]] = point2;

                    Debug.Log($"  Expanding vertically:\n" +
                              $"    Corner: {original} > {currentPoint}\n" +
                              $"    Opposite 1: {original1} > {point1}\n" +
                              $"    Opposite 2: {original2} > {point2}");
                }
            }
            else if (currentCorner == Corner.InnerBottomRight) {

                Debug.Log($"Expanding With Inner Bottom Right Corner Point: {currentPoint}");

                int[] horizontalAcrossPointIndexes = null;
                float minHorizontalDistance = float.PositiveInfinity;

                int[] verticalAcrossPointIndexes = null;
                float minVerticalDistance = float.PositiveInfinity;

                for (int searchIndex1 = 0; searchIndex1 < newPoints.Count; searchIndex1++) {
                    int searchIndex2 = (searchIndex1 + 1) % newPoints.Count;

                    Vector2 point1 = newPoints[searchIndex1];
                    Vector2 point2 = newPoints[searchIndex2];

                    Debug.Log($"  Search points {point1} and {point2}");

                    bool pointsMakeVerticalLine = point1.x == point2.x;
                    bool pointsMakeHorizontalLine = point1.y == point2.y;
                    if (pointsMakeVerticalLine) {

                        if (currentPoint.x <= point1.x) {
                            continue;
                        }

                        float minY = Mathf.Min(point1.y, point2.y);
                        float maxY = Mathf.Max(point1.y, point2.y);
                        bool betweenYPositions = minY <= currentPoint.y && maxY >= currentPoint.y;
                        if (betweenYPositions) {
                            Debug.Log($"    Points are across horizontally");

                            float horizontalDistance = Mathf.Abs(point1.x - currentPoint.x);
                            if (horizontalDistance < minHorizontalDistance) {
                                horizontalAcrossPointIndexes = new int[] { searchIndex1, searchIndex2 };
                                minHorizontalDistance = horizontalDistance;
                            }
                        }
                    }
                    else if (pointsMakeHorizontalLine) {

                        if (currentPoint.y >= point1.y) {
                            continue;
                        }

                        float minX = Mathf.Min(point1.x, point2.x);
                        float maxX = Mathf.Max(point1.x, point2.x);
                        bool betweenXPositions = minX <= currentPoint.x && maxX >= currentPoint.x;
                        if (betweenXPositions) {
                            Debug.Log($"    Points are across vertically");

                            float verticalDistance = Mathf.Abs(point1.y - currentPoint.y);
                            if (verticalDistance < minVerticalDistance) {
                                verticalAcrossPointIndexes = new int[] { searchIndex1, searchIndex2 };
                                minVerticalDistance = verticalDistance;
                            }
                        }
                    }
                    else {
                        Debug.LogError("Points don't make vertical or horizontal line!");
                        continue;
                    }
                }

                if (minVerticalDistance == float.PositiveInfinity) {
                    Debug.LogError("Did not find points that were across vertically!");
                    return newPoints;
                }

                if (minHorizontalDistance == float.PositiveInfinity) {
                    Debug.LogError("Did not find points that were across horizontally!");
                    return newPoints;
                }

                if (minHorizontalDistance < minWidth) {
                    float totalExpansion = minWidth - minHorizontalDistance;
                    float oneSideExpansion = Mathf.CeilToInt(totalExpansion / 2f);

                    Vector2 original = currentPoint;
                    currentPoint.x += oneSideExpansion;
                    newPoints[currentIndex] = currentPoint;

                    nextPoint.x += oneSideExpansion;
                    newPoints[nextIndex] = nextPoint;

                    Vector2 point1 = newPoints[horizontalAcrossPointIndexes[0]];
                    Vector2 original1 = point1;
                    point1.x -= oneSideExpansion;
                    newPoints[horizontalAcrossPointIndexes[0]] = point1;

                    Vector2 point2 = newPoints[horizontalAcrossPointIndexes[1]];
                    Vector2 original2 = point2;
                    point2.x -= oneSideExpansion;
                    newPoints[horizontalAcrossPointIndexes[1]] = point2;

                    Debug.Log($"  Expanding horizontally:\n" +
                              $"    Corner: {original} > {currentPoint}\n" +
                              $"    Opposite 1: {original1} > {point1}\n" +
                              $"    Opposite 2: {original2} > {point2}");
                }

                if (minVerticalDistance < minHeight) {
                    float totalExpansion = minHeight - minVerticalDistance;
                    float oneSideExpansion = Mathf.CeilToInt(totalExpansion / 2f);

                    Vector2 original = currentPoint;
                    currentPoint.y -= oneSideExpansion;
                    newPoints[currentIndex] = currentPoint;

                    previousPoint.y -= oneSideExpansion;
                    newPoints[prevIndex] = previousPoint;

                    Vector2 point1 = newPoints[verticalAcrossPointIndexes[0]];
                    Vector2 original1 = point1;
                    point1.y += oneSideExpansion;
                    newPoints[verticalAcrossPointIndexes[0]] = point1;

                    Vector2 point2 = newPoints[verticalAcrossPointIndexes[1]];
                    Vector2 original2 = point2;
                    point2.y += oneSideExpansion;
                    newPoints[verticalAcrossPointIndexes[1]] = point2;

                    Debug.Log($"  Expanding vertically:\n" +
                              $"    Corner: {original} > {currentPoint}\n" +
                              $"    Opposite 1: {original1} > {point1}\n" +
                              $"    Opposite 2: {original2} > {point2}");
                }
            }
        }

        return newPoints;
    }

    private List<Vector2> AddPadding(List<Vector2> points) {
        List<Vector2> paddedPoints = new List<Vector2>();

        for (int pointIndex = 0; pointIndex < roomCollider.points.Length; pointIndex++) {

            // Use modulo to wrap around
            int prevIndex = (pointIndex - 1 + points.Count) % points.Count;
            int nextIndex = (pointIndex + 1) % points.Count;

            Vector2 previousPoint = points[prevIndex];
            Vector2 currentPoint = points[pointIndex];
            Vector2 nextPoint = points[nextIndex];

            float offsetValue = 2f;
            Vector2 offset = Vector2.zero;

            Corner corner = GetPointCorner(previousPoint, currentPoint, nextPoint);
            if (corner == Corner.OuterTopLeft || corner == Corner.InnerTopLeft) {
                offset = new Vector2(-offsetValue, offsetValue);
            }
            else if (corner == Corner.OuterTopRight || corner == Corner.InnerTopRight) {
                offset = new Vector2(offsetValue, offsetValue);
            }
            else if (corner == Corner.OuterBottomRight || corner == Corner.InnerBottomRight) {
                offset = new Vector2(offsetValue, -offsetValue);
            }
            else if (corner == Corner.OuterBottomLeft || corner == Corner.InnerBottomLeft) {
                offset = new Vector2(-offsetValue, -offsetValue);
            }

            paddedPoints.Add(currentPoint + offset);
        }

        return paddedPoints;
    }

    private Corner GetPointCorner(Vector2 previousPoint, Vector2 currentPoint, Vector2 nextPoint) {

        if (currentPoint.x > previousPoint.x && currentPoint.y > nextPoint.y) {
            return Corner.OuterTopRight;
        }

        if (currentPoint.y < previousPoint.y && currentPoint.x < nextPoint.x) {
            return Corner.InnerTopRight;
        }

        if (currentPoint.y < previousPoint.y && currentPoint.x > nextPoint.x) {
            return Corner.OuterBottomRight;
        }

        if (currentPoint.x < previousPoint.x && currentPoint.y > nextPoint.y) {
            return Corner.InnerBottomRight;
        }

        if (currentPoint.y > previousPoint.y && currentPoint.x < nextPoint.x) {
            return Corner.OuterTopLeft;
        }

        if (currentPoint.x > previousPoint.x && currentPoint.y < nextPoint.y) {
            return Corner.InnerTopLeft;
        }

        if (currentPoint.x < previousPoint.x && currentPoint.y < nextPoint.y) {
            return Corner.OuterBottomLeft;
        }

        if (currentPoint.y > previousPoint.y && currentPoint.x > nextPoint.x) {
            return Corner.InnerBottomLeft;
        }

        Debug.LogError("Could not find point corner type!");
        return default;
    }

    #endregion


    private void AddPoint(PolygonCollider2D polygonCollider, Vector2 point) {

        // Add the new point to the list
        List<Vector2> pointsList = polygonCollider.points.ToList();
        pointsList.Add(point);
        polygonCollider.points = pointsList.ToArray();

        // Mark the object as dirty so the editor knows it has changed
        EditorUtility.SetDirty(polygonCollider);
    }

    private Vector3Int TopLeftTilePosition() {
        int minXPos = Mathf.Min(topWallsTilemap.cellBounds.xMin, botWallsTilemap.cellBounds.xMin);
        int minYPos = Mathf.Min(topWallsTilemap.cellBounds.yMin, botWallsTilemap.cellBounds.yMin);
        int maxXPos = Mathf.Max(topWallsTilemap.cellBounds.xMax, botWallsTilemap.cellBounds.xMax);
        int maxYPos = Mathf.Max(topWallsTilemap.cellBounds.yMax, botWallsTilemap.cellBounds.yMax);
        for (int y = maxYPos; y >= minYPos; y--) {
            for (int x = minXPos; x <= maxXPos; x++) {
                Vector3Int posToCheck = new(x, y);
                if (TileAtPosition(posToCheck)) {
                    return posToCheck;
                }
            }
        }
        Debug.LogError("No tile found in wall tile maps!");
        return Vector3Int.zero;
    }

    private Vector2 TopLeftTileOuterPosition() {
        Vector2 outPos = (Vector3)TopLeftTilePosition();
        outPos.y++;
        return outPos;
    }

    private Vector2 TopRightTileOuterPosition() {
        int minXPos = Mathf.Min(topWallsTilemap.cellBounds.xMin, botWallsTilemap.cellBounds.xMin);
        int minYPos = Mathf.Min(topWallsTilemap.cellBounds.yMin, botWallsTilemap.cellBounds.yMin);
        int maxXPos = Mathf.Max(topWallsTilemap.cellBounds.xMax, botWallsTilemap.cellBounds.xMax);
        int maxYPos = Mathf.Max(topWallsTilemap.cellBounds.yMax, botWallsTilemap.cellBounds.yMax);
        for (int y = maxYPos; y >= minYPos; y--) {
            for (int x = maxXPos; x >= minXPos; x--) {
                Vector3Int posToCheck = new(x, y);
                if (TileAtPosition(posToCheck)) {
                    Vector2 outsideTilePos = (Vector3)posToCheck;
                    outsideTilePos.x++;
                    outsideTilePos.y++;
                    return outsideTilePos;
                }
            }
        }
        Debug.LogError("No tile found in wall tile maps!");
        return Vector2.zero;
    }

    private Vector2 BottomLeftTileOuterPosition() {
        int minXPos = Mathf.Min(topWallsTilemap.cellBounds.xMin, botWallsTilemap.cellBounds.xMin);
        int minYPos = Mathf.Min(topWallsTilemap.cellBounds.yMin, botWallsTilemap.cellBounds.yMin);
        int maxXPos = Mathf.Max(topWallsTilemap.cellBounds.xMax, botWallsTilemap.cellBounds.xMax);
        int maxYPos = Mathf.Max(topWallsTilemap.cellBounds.yMax, botWallsTilemap.cellBounds.yMax);
        for (int y = minYPos; y <= maxYPos; y++) {
            for (int x = minXPos; x <= maxXPos; x++) {
                Vector3Int posToCheck = new(x, y);
                if (TileAtPosition(posToCheck)) {
                    Vector2 outsideTilePos = (Vector3)posToCheck;
                    return outsideTilePos;
                }
            }
        }
        Debug.LogError("No tile found in wall tile maps!");
        return Vector2.zero;
    }

    private Vector2 BottomRightTileOuterPosition() {
        int minXPos = Mathf.Min(topWallsTilemap.cellBounds.xMin, botWallsTilemap.cellBounds.xMin);
        int minYPos = Mathf.Min(topWallsTilemap.cellBounds.yMin, botWallsTilemap.cellBounds.yMin);
        int maxXPos = Mathf.Max(topWallsTilemap.cellBounds.xMax, botWallsTilemap.cellBounds.xMax);
        int maxYPos = Mathf.Max(topWallsTilemap.cellBounds.yMax, botWallsTilemap.cellBounds.yMax);
        for (int y = minYPos; y <= maxYPos; y++) {
            for (int x = maxXPos; x >= minXPos; x--) {
                Vector3Int posToCheck = new(x, y);
                if (TileAtPosition(posToCheck)) {
                    Vector2 outsideTilePos = (Vector3)posToCheck;
                    outsideTilePos.x++;
                    return outsideTilePos;
                }
            }
        }
        Debug.LogError("No tile found in wall tile maps!");
        return Vector2.zero;
    }
}

#endif