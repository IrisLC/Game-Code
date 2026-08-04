using UnityEngine;

public class Node
{
    public Vector3 worldPosition;
    public bool isObstacle;

    public int x;
    public int y;

    public Node(bool _isObstacle, Vector3 _worldPos, int _x, int _y)
    {
        isObstacle = _isObstacle;
        worldPosition = _worldPos;
        x = _x;
        y = _y;
    }
}
