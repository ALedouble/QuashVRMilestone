using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
    // public static LevelScript Instance;

    [SerializeField]
    public string nameLevel;

    public int totalRows = 10; //y

    public int totalColumns = 10; //x

    public float CellSize = 0.3f;
    private readonly Color normalColor = Color.red;
    private readonly Color selectedColor = Color.green;

    public List<Vector3> editorSpace;

    public float xGridPlacement = 0f;

    public float yGridPlacement = 1f;

    public float zGridPlacement;

    [SerializeField]
    public Sprite background;



    public LevelsScriptable[] allLevels;

    public LevelsScriptable selectedLevel;
    public LevelSettings levelCategories;

    public PresetScriptable[] colorPresets;
    //private string presetPath = "Assets/ScriptableObjects/ColorPresets";

    public BrickTypesScriptable[] brickPresets;
    //private string brickPresetPath = "Assets/ScriptableObjects/BrickPresets";

    public int colorPresetSelected = 0;
    public int brickPresetSelected = 0;

    public GameObject[] bricksOnScreen;
    public int bricksOnLayer;
    public int numberOfNeutralBrick;
    public int numberOfColoredBrick01;
    public int numberOfColoredBrick02;

    public GameObject brickPrefab;
    public List<Vector3> brickWaypoints;

    public Wall currentLayer;
    public int selectedLayer;
    public int numberOfLayers;
    public int totalLayersDisplayed;

    //private GameObject prefabBase;
    //private string prefabPath = "Assets/Prefabs/Bricks";



    /// <summary>
    /// Return Width space for the editor
    /// </summary>
    /// <returns></returns>
    public float maxWidthSpace()
    {
        return editorSpace[1].x - editorSpace[0].x;
    }

    /// <summary>
    /// Return result value/distance from the first column 'til the last one 
    /// </summary>
    /// <returns></returns>
    private float colSpace()
    {
        return CellSize * (float)totalColumns;
    }

    public float maxHeightSpace()
    {
        return editorSpace[1].y - editorSpace[0].y;
    }

    private float rowSpace()
    {
        return CellSize * (float)totalRows;
    }

    private float WidthOffset()
    {
        return (maxWidthSpace() - colSpace()) / 2;
    }



    private void GridFrameGizmo(int cols, int rows)
    {
        //Debug.Log("Draw Grid Frame");
        /* LEFT */
        Gizmos.DrawLine(new Vector3(xGridPlacement + WidthOffset(),
            yGridPlacement,
            zGridPlacement),

            new Vector3(xGridPlacement + WidthOffset(),
            (rows * CellSize) + yGridPlacement,
            zGridPlacement));

        ///* RIGHT */
        Gizmos.DrawLine(new Vector3((cols * CellSize) + xGridPlacement + WidthOffset(),
                    yGridPlacement,
                    zGridPlacement),

                    new Vector3((cols * CellSize) + xGridPlacement + WidthOffset(),
                    (rows * CellSize) + yGridPlacement,
                    zGridPlacement));

        ///* BOTTOM */
        Gizmos.DrawLine(new Vector3(xGridPlacement + WidthOffset(),
            yGridPlacement,
            zGridPlacement),

            new Vector3((cols * CellSize) + xGridPlacement + WidthOffset(),
            yGridPlacement,
            zGridPlacement));



        ///* UP */
        Gizmos.DrawLine(new Vector3(xGridPlacement + WidthOffset(),
            (rows * CellSize) + yGridPlacement,
            zGridPlacement),

            new Vector3((cols * CellSize) + xGridPlacement + WidthOffset(),
            (rows * CellSize) + yGridPlacement,
            zGridPlacement));
    }

    private void GridGizmo(int cols, int rows)
    {
        for (int i = 1; i < cols; i++)
        {
            //COLUMNS
            Gizmos.DrawLine(new Vector3(xGridPlacement + WidthOffset() + (i * CellSize),
                yGridPlacement,
                zGridPlacement),

                new Vector3(xGridPlacement + WidthOffset() + (i * CellSize),
                (rows * CellSize) + yGridPlacement,
                zGridPlacement));
        }


        for (int j = 1; j < rows; j++)
        {
            //ROWS
            Gizmos.DrawLine(new Vector3(xGridPlacement + WidthOffset(),
            ((j * CellSize) + yGridPlacement),
            zGridPlacement),

            new Vector3((xGridPlacement + WidthOffset() + (cols * CellSize)),
            ((j * CellSize) + yGridPlacement),
            zGridPlacement));
        }
    }

    private void OnDrawGizmosSelected()
    {
        //editorSpace = new List<Vector3>(2);
        //editorSpace[0] = new Vector3(0, 1, 0);
        //editorSpace[1] = new Vector3(5, 7, 0);

        if (editorSpace.Count > 1)
        {
            Color oldColor = Gizmos.color;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = normalColor;
            GridGizmo(totalColumns, totalRows);
            GridFrameGizmo(totalColumns, totalRows);

            Gizmos.color = Color.green;
            WaypointConnections();

            Gizmos.color = oldColor;
            Gizmos.matrix = oldMatrix;
        }

    }

    private void WaypointConnections()
    {
        //Debug.Log("Draw Waypoint Gizmo");

        for (int x = 0; x < currentLayer.wallBricks.Count; x++)
        {
            if (currentLayer.wallBricks[x].isMoving)
            {
                if (currentLayer.wallBricks[x].waypointsStorage.Count > 1)
                {
                    for (int j = 0; j < currentLayer.wallBricks[x].waypointsStorage.Count - 1; j++)
                    {
                        Gizmos.DrawLine(new Vector3(currentLayer.wallBricks[x].waypointsStorage[j].x,
                    currentLayer.wallBricks[x].waypointsStorage[j].y,
                    currentLayer.wallBricks[x].waypointsStorage[j].z),

                    new Vector3(currentLayer.wallBricks[x].waypointsStorage[j + 1].x,
                    currentLayer.wallBricks[x].waypointsStorage[j + 1].y,
                    currentLayer.wallBricks[x].waypointsStorage[j + 1].z));
                    }

                    Gizmos.DrawLine(new Vector3(currentLayer.wallBricks[x].waypointsStorage[currentLayer.wallBricks[x].waypointsStorage.Count - 1].x,
                    currentLayer.wallBricks[x].waypointsStorage[currentLayer.wallBricks[x].waypointsStorage.Count - 1].y,
                    currentLayer.wallBricks[x].waypointsStorage[currentLayer.wallBricks[x].waypointsStorage.Count - 1].z),

                    new Vector3(currentLayer.wallBricks[x].waypointsStorage[0].x,
                    currentLayer.wallBricks[x].waypointsStorage[0].y,
                    currentLayer.wallBricks[x].waypointsStorage[0].z));
                }

            }
        }
    }



    /// <summary>
    /// Receive a Vector3 and return a vector 3 where x and y correspond to cols and rows coordinates
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3 WorldToGridCoordinates(Vector3 point)
    {
        Vector3 gridPoint = new Vector3(
            (int)((point.x - xGridPlacement - WidthOffset()) / CellSize),
             (int)((point.y - yGridPlacement) / CellSize),
             zGridPlacement);

        return gridPoint;
    }

    /// <summary>
    /// Receive col and row position and return Vector3 corresponding to world coordinates(y = 0)
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public Vector3 GridToWorldPoint(int col, int row)
    {
        Vector3 worldPoint = new Vector3(
            xGridPlacement + WidthOffset() + (col * CellSize) + (CellSize / 2),
            yGridPlacement + (row * CellSize) + (CellSize / 2),
            zGridPlacement);
        return worldPoint;
    }



    /// <summary>
    /// Booleans receive a vector 3 and return true or false if coordinates are inside the grid
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool IsInsideGridBounds(Vector3 point)
    {
        float minX = xGridPlacement - WidthOffset();
        float maxX = (totalColumns * CellSize) - minX;
        Debug.Log("min : " + minX + " max : " + maxX);

        float minY = yGridPlacement;
        float maxY = minY + totalRows * CellSize;

        return (point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY);
    }

    public bool IsInsideGridBounds(int col, int row)
    {
        return (col >= 0 && col < totalColumns && row >= 0 && row < totalRows);
    }
}
