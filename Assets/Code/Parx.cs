using UnityEngine;

public class Parx : MonoBehaviour
{
    // --- --- ---
    //
    // Protyping for 'Parks in the Park' mini-game
    //
    // --- --- ---

    public static Parx instance;


    public int gridSize = 5;

    [Space]

    public Transform gridParent;
    public Transform borderParent;


    private int s;

    private int[,] _grid;
    private int[,] _clrs;
    private int[] _bordr;
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitGrid(gridSize);
        InitGridBlox(gridSize);
        InitGridBorder();

        PrintGrid();
    }

    // GAME LOGIC

    public bool PlaceTree(int x, int y, int v)
    {
        // Debug.Log(" - Placing " + v + " at [ " + x + " , " + y + "] ");

        if (_grid[x,y] == 0 || _grid[x,y] == v)
        {
            int m = _grid[x,y] == v ? 1 : -1;

            for ( int iX = 0; iX < s; iX++ ) {_grid[iX,y] += m; } // Mark Row
            for ( int iY = 0; iY < s; iY++ ) {_grid[x,iY] += m; } // Mark Column

            if ( x > 0   && y > 0  ) {_grid[x-1,y-1] += m; } // Top Left
            if ( x < s-1 && y > 0  ) {_grid[x+1,y-1] += m; } // Top Right
            if ( x > 0   && y < s-1) {_grid[x-1,y+1] += m; } // Bottom Left
            if ( x < s-1 && y < s-1) {_grid[x+1,y+1] += m; } // Bottom Right

            _grid[x,y] = m == 1 ? 0 : v; // Mark Selection

            _blox[x,y].ToggleFrontPlate(m != 1);

            UpdateBorders();

            PrintGrid();

            return true;
        }

        return false;
    }

    private void UpdateBorders()
    {
        // Check Rows
        for ( int y = 0; y < s; y++ )
        {
            int count = 0;
            for ( int x = 0; x < s; x++ )
            {
                if ( _grid[x,y] > 0 ) count++;
            }

            _bordr[s+y]     = count > 0 ? 1 : 0; _brdr[ s   +y].Toggle(count > 0);
            _bordr[(s*2)+y] = count > 0 ? 1 : 0; _brdr[(s*2)+y].Toggle(count > 0);
        }

        // Check Columns
        for ( int x = 0; x < s; x++ )
        {
            int count = 0;
            for ( int y = 0; y < s; y++ )
            {
                if ( _grid[x,y] > 0 ) count++;
            }

            _bordr[x]       = count > 0 ? 1 : 0; _brdr[      x].Toggle(count > 0);
            _bordr[(s*3)+x] = count > 0 ? 1 : 0; _brdr[(s*3)+x].Toggle(count > 0);
        }
    }

    private void InitGrid(int gridSize = 5)
    {
            s  = gridSize;
        _grid  = new int[s,  s];
        _clrs  = new int[s,  s];
        _bordr = new int[s * 4];

        for ( int x = 0; x < s; x++ )
        {
            for ( int y = 0; y < s; y++ )
            {
                _grid[x,y] = 0;
                _clrs[x,y] = 0;
            }
        }
    }

    private void PrintGrid()
    {
        string outputString = "\n - - -\n";

        for ( int y = 0; y < s; y++ )
        {
            // Print Top Border
            if ( y == 0 )
            {
                outputString += "   ";
                for ( int i = 0; i < s; i++ )
                {
                    outputString += "(" + _bordr[i] + ")";
                }
                outputString += "\n";
            }

            for ( int x = 0; x < s; x++ )
            {
                if ( x == 0 ) { outputString += "(" + _bordr[s+y] + ") "; }

                    // Print Square at x,y:

                    string blockTxt = _grid[x,y] == 0 ? " " : _grid[x,y] < 0 ? "•" : _grid[x,y].ToString();

                    _blox[x,y].SetText(blockTxt);

                    outputString += "[" + blockTxt + "] ";

                if ( x == s-1 ) { outputString += "(" + _bordr[(s*2)+y] + ") "; }
            }

            // Print Bottom Border
            if ( y == s-1 )
            {
                outputString += "\n   ";
                for ( int i = s*3; i < s*4; i++ )
                {
                    outputString += "(" + _bordr[i] + ")";
                }
            }

            outputString += "\n";
        }

        outputString += " - - -";
        
        Debug.Log(outputString);
    }

    public void ClearGrid()
    {
        for ( int x = 0; x < s; x++ )
        {
            for ( int y = 0; y < s; y++ )
            {
                _grid[x,y] = 0;
                _blox[x,y].ToggleFrontPlate(false);
            }
        }

        for (int i = 0; i < _brdr.Length; i++ )
        {
            _brdr[i].Toggle(false);
        }

        PrintGrid();
    }

    public void RegenerateGrid()
    {
        for ( int x = 0; x < s; x++ )
        {
            for ( int y = 0; y < s; y++ )
            {
                GameObject.Destroy(_blox[x,y].gameObject);
            }
        }

        for (int i = 0; i < _brdr.Length; i++ )
        {
            GameObject.Destroy(_brdr[i].gameObject);
        }

        InitGrid(gridSize);
        InitGridBlox(gridSize);
        InitGridBorder();

        PrintGrid();
    }

    // PRESENTATION
    [Header("Grid Spawn")]
    public GameObject parxBlock;
    public GameObject parxBordr;
    [Space]
    public float spawnStartX;
    public float spawnStartY;
    [Space]
    public float spawnDeltaX;
    public float spawnDeltaY;

    private ParxBlock [,] _blox;
    private ParxBorder[ ] _brdr;

    public void InitGridBlox(int gridSize = 5)
    {
            s = gridSize;
        _blox = new ParxBlock[s, s];

        for ( int x = 0; x < s; x++ )
        {
            for ( int y = 0; y < s; y++ )
            {
                float xPos = spawnStartX + (x*spawnDeltaX);
                float yPos = spawnStartY - (y*spawnDeltaY);

                _blox[x,y] = SpawnNewBlock(new Vector3(xPos, 0.0f,  yPos), x, y);
                _blox[x,y].Init(x,y);
            }
        }

        UpdateCameraControllerTarget();
    }
    
    private void UpdateCameraControllerTarget()
    {
        int i = gridSize - 1;

        CameraController.instance.target1 = _blox[0,0].transform;
        CameraController.instance.target2 = _blox[i,i].transform;
    }

    private ParxBlock SpawnNewBlock(Vector3 spawnPosition, int x, int y)
    {
        GameObject newBlock = Instantiate(parxBlock);

        newBlock.transform.parent   = gridParent;
        newBlock.transform.position = spawnPosition;
        newBlock.name               = "ParxBlock (" + (x+1) + " , " + (y+1) + ")";

        return newBlock.GetComponent<ParxBlock>();
    }

    private void InitGridBorder()
    {
        _brdr = new ParxBorder[s*4];
        
        // Spawn Rows
        for ( int y = 0; y < s; y++ )
        {   
            float x1 = spawnStartX - 0.5f;
            float x2 = x1 + (s*spawnDeltaX) - 0.3f;
            float yP = spawnStartY - (y*spawnDeltaY);

            _brdr[s+y]     = SpawnNewBorderBulb(new Vector3(x1, 0.0f, yP), new Vector3(0.0f, 0.0f, 90.0f), -1, y);
            _brdr[(s*2)+y] = SpawnNewBorderBulb(new Vector3(x2, 0.0f, yP), new Vector3(0.0f, 0.0f, 90.0f),  s, y);
        }
        
        // Spawn Columns
        for ( int x = 0; x < s; x++ )
        {   
            float y1 = spawnStartY + 0.5f;
            float y2 = y1 - (s*spawnDeltaY) + 0.3f;
            float xP = spawnStartX + (x*spawnDeltaX);

            _brdr[x]       = SpawnNewBorderBulb(new Vector3(xP, 0.0f, y1), new Vector3(0.0f, 90.0f, 90.0f), x, -1);
            _brdr[(s*3)+x] = SpawnNewBorderBulb(new Vector3(xP, 0.0f, y2), new Vector3(0.0f, 90.0f, 90.0f), x,  s);
        }
    }

    private ParxBorder SpawnNewBorderBulb(Vector3 spawnPosition, Vector3 spawnRot, int x, int y)
    {
        GameObject newBulb = Instantiate(parxBordr);

        newBulb.transform.parent            = borderParent;
        newBulb.transform.position          = spawnPosition;
        newBulb.transform.localEulerAngles  = spawnRot;
        newBulb.name                        = "ParxBulb (" + (x+1) + " , " + (y+1) + ")";

        ParxBorder bulbRef = newBulb.GetComponent<ParxBorder>();
        bulbRef.Toggle(false);
        return bulbRef;
    }

}
