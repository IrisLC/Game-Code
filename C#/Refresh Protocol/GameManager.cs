using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Node[,] playGrid;
    [SerializeField] private Vector2 gridWorldSize;
    [SerializeField] private Vector2 gridCount;
    [SerializeField] private bool useCount;
    [SerializeField] private float nodeRadius;

    private float nodeDiameter;
    int gridSizeX, gridSizeY;

    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask startMask;
    [SerializeField] private LayerMask goalMask;
    [SerializeField] private LayerMask keyMask;

    private Node start;
    private Node goal;
    private Node key;

    private bool needsKey;
    private bool hasKey;
    [SerializeField] GameObject keyObject;
    [SerializeField] GameObject keyBlocker;

    private int currentX;
    private int currentY;

    [SerializeField] private GameObject player;

    public enum Inputs
    {
        Up, Down, Left, Right, Wait, Interact, None
    }

    public List<Inputs> playerInputs;
    public int inputIndex;

    private int errorCount;

    public bool isPlaying;

    [SerializeField] private InputReader ir;

    [SerializeField] private GameObject endScreen;

    private UIManager uim;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uim = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        ir = GameObject.FindGameObjectWithTag("InputReader").GetComponent<InputReader>();
        nodeDiameter = nodeRadius * 2;
        if (useCount)
        {
            gridWorldSize.x = gridCount.x * nodeDiameter;
            gridWorldSize.y = gridCount.y * nodeDiameter;
        }

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
        Reset();
        
    }

    public void readInputs()
    {
        playerInputs = ir.readInputs();
        InvokeRepeating("Step", 0f, 1f);
        isPlaying = true;
    }

    void Step()
    {
        int prevX = currentX;
        int prevY = currentY;
        Inputs nextInput = playerInputs[inputIndex];
        switch (nextInput)
        {
            case Inputs.Left:
                if (currentX != 0)
                {
                    if (playGrid[currentX - 1, currentY].isObstacle)
                    {
                        player.GetComponent<Player>().bump(playGrid[prevX, prevY].worldPosition, playGrid[currentX - 1, currentY].worldPosition);
                        break;
                    }
                    currentX--;
                }

                break;
            case Inputs.Right:
                if (currentX != gridSizeX - 1)
                {
                    if (playGrid[currentX + 1, currentY].isObstacle)
                    {
                        player.GetComponent<Player>().bump(playGrid[prevX, prevY].worldPosition, playGrid[currentX + 1, currentY].worldPosition);
                        break;
                    }
                    currentX++;
                }
                break;
            case Inputs.Up:
                if (currentY != gridSizeY - 1)
                {
                    if (playGrid[currentX, currentY + 1].isObstacle)
                    {
                        player.GetComponent<Player>().bump(playGrid[prevX, prevY].worldPosition, playGrid[currentX, currentY + 1].worldPosition);
                        break;
                    }
                    currentY++;
                }
                break;
            case Inputs.Down:
                if (currentY != 0)
                {
                    if (playGrid[currentX, currentY - 1].isObstacle)
                    {
                        player.GetComponent<Player>().bump(playGrid[prevX, prevY].worldPosition, playGrid[currentX, currentY - 1].worldPosition);
                        break;
                    }
                    currentY--;
                }
                break;
            case Inputs.Interact:
                if (currentX == key.x && currentY == key.y)
                {
                    hasKey = true;
                    
                    keyObject.SetActive(false);
                    keyBlocker.SetActive(false);
                }
                errorCount--;
                break; 
            case Inputs.Wait:
                errorCount--;
                break;
        }

        uim.activeIndex = inputIndex;

        inputIndex++;
        if (inputIndex == playerInputs.Count) inputIndex = 0;
        //If the player gets stuck
        if (prevX == currentX && prevY == currentY)
        {
            errorCount++;

            if (errorCount >= 5)
            {


                Reset();
                return;
            }
            return;
        }
        else
        {
            errorCount = 0;
        }
        
        
        //player.transform.position = playGrid[currentX, currentY].worldPosition;
        player.GetComponent<Player>().newPosition(playGrid[prevX, prevY].worldPosition, playGrid[currentX, currentY].worldPosition);

        if (currentX == goal.x && currentY == goal.y)
        {
            if (needsKey && !hasKey) return;
            StartCoroutine(levelComplete());
        }
    }

    public void Reset()
    {
        CancelInvoke("Step");
        player.transform.position = start.worldPosition;
        player.GetComponent<Player>().endMove();
        currentX = start.x;
        currentY = start.y;
        inputIndex = 0;

        if (needsKey)
        {
            keyBlocker.SetActive(true);
            keyObject.SetActive(true);
            hasKey = false;
        }

        isPlaying = false;


    }

    private IEnumerator levelComplete()
    {
        CancelInvoke("Step");
        
        yield return new WaitForSeconds(1f);
        player.GetComponent<Player>().endMove();
        Debug.Log("Win");
        endScreen.SetActive(true);
        isPlaying = false;
    }



    void CreateGrid()
    {
        playGrid = new Node[gridSizeX, gridSizeY];
        Vector2 transPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 WorldBottomLeft = transPos - (Vector2.right * (gridWorldSize.x / 2)) - (Vector2.up * (gridWorldSize.y / 2));
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                Vector2 worldPoint = WorldBottomLeft + Vector2.right * (i * nodeDiameter + nodeRadius) + Vector2.up * (j * nodeDiameter + nodeRadius);
                bool Obstacle = Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask);
                playGrid[i, j] = new Node(Obstacle, worldPoint, i, j);
                //Add the start and goal nodes if it collides with the start/goal objects
                if (Physics.CheckSphere(worldPoint, nodeRadius, startMask))
                {
                    start = playGrid[i, j];
                }
                else if (Physics.CheckSphere(worldPoint, nodeRadius, goalMask))
                {
                    goal = playGrid[i, j];
                }
                else if (Physics.CheckSphere(worldPoint, nodeRadius, keyMask))
                {
                    keyObject = GameObject.FindGameObjectWithTag("Key");
                    keyBlocker = GameObject.FindGameObjectWithTag("KeyBlocker");
                    key = playGrid[i, j];
                    keyBlocker.SetActive(true);
                    needsKey = true;
                    
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, gridWorldSize);
        if (playGrid != null)
        {
            foreach (Node n in playGrid)
            {
                Gizmos.color = Color.white;
                if (n.isObstacle) Gizmos.color = Color.black;

                if (n == start) Gizmos.color = Color.green;
                if (n == goal) Gizmos.color = Color.gold;


                Gizmos.DrawCube(n.worldPosition, new Vector3(nodeDiameter, nodeDiameter, nodeDiameter));
                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(n.worldPosition, new Vector3(nodeDiameter, nodeDiameter, nodeDiameter));
            }

            


        }
        
        
    }
}
