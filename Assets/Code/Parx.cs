using System;
using System.Collections.Generic;
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

    public bool autoDie = true;

    [Header("Audio")]
    public AudioSource aSourceSFX;
    [Space]
    public AudioClip sfxClick;
    public AudioClip sfxHover;
    public AudioClip sfxMarkr;

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
        InitGrid();
        InitColors();
        InitGridBlox();
        InitGridBorder();

        UpdateGridPresentation();
    }

    // GAME LOGIC

    public bool PlaceTree(int x, int y, int v, bool addToUndo = false)
    {
        // if (_grid[x,y] == 0 || _grid[x,y] == v)
        {
            int m = _grid[x,y] >= 3 ? 1 : -1;

            _grid[x,y] = m == 1 ? 0 : v; // Mark Selection

            if (m == 1 && autoDie) 
            CrossMark(x, y, false);

            AutoMarkBoard();
            ReviewMarkers();
            UpdateBorders();
            
            ToggleGlows(_clrs[x,y]);
            UpdateGridPresentation();

            if ( IsBoardAllGreen() )
            {
                ParxManager.instance.nextButton.Enable(true);
            }

            if ( addToUndo )
            {
                aSourceSFX.PlayOneShot(sfxClick);

                // TO DO - Implement UNDO 
            }

            return true;
        }

        // return false;
    }

    public void PlaceMarker(int x, int y, bool addToUndo = false)
    {
        _grid[x,y] = _grid[x,y] == 0 ? -1 : _grid[x,y] == -1 ? 0 : _grid[x,y];

        UpdateGridPresentation();

        if ( addToUndo )
        {
            aSourceSFX.PlayOneShot(sfxMarkr);
            
            // TO DO - Implement UNDO 
        }
    }

    public void AutoMarkBoard()
    {
        if ( autoDie )
        {
            for ( int x = 0; x < s; x++ )
            {
                for ( int y = 0; y < s; y++ )
                {
                    if ( _grid[x,y] > 0 )
                    {
                        CrossMark(x, y, true);
                    }
                }
            }
        }
    }

    public void CrossMark(int x, int y, bool toggle)
    {
        for ( int iX = 0; iX < s; iX++ ) {ForceToggleMarker(iX,y, toggle); } // Mark Row
        for ( int iY = 0; iY < s; iY++ ) {ForceToggleMarker(x,iY, toggle); } // Mark Column

        if ( x > 0   && y > 0  ) {ForceToggleMarker(x-1,y-1, toggle); } // Top Left
        if ( x < s-1 && y > 0  ) {ForceToggleMarker(x+1,y-1, toggle); } // Top Right
        if ( x > 0   && y < s-1) {ForceToggleMarker(x-1,y+1, toggle); } // Bottom Left
        if ( x < s-1 && y < s-1) {ForceToggleMarker(x+1,y+1, toggle); } // Bottom Right
    }

    public void ForceToggleMarker(int x, int y, bool on, bool overrideTrees = false)
    {
        if ( _grid[x,y] > 0 && overrideTrees == false ) return;

        _grid[x,y] = on ? -1 : 0;
    }

    private void UpdateBorders()
    {
        // Check Rows
        for ( int y = 0; y < s; y++ )
        {
            int tris = 0;
            int dots = 0;
            int reds = 0;

            for ( int x = 0; x < s; x++ )
            {
                if ( _grid[x,y] == 3 ) tris++;
                if ( _grid[x,y] == 4 ) reds++;
                if ( _grid[x,y]  < 0 ) dots++;
            }

            _bordr[s+y]     = (tris == 1 && reds == 0) ? 1 : 0; 
            _bordr[(s*2)+y] = (tris == 1 && reds == 0) ? 1 : 0;

            int tgl = reds > 0 ? 9 : tris == 1 ? 1 : dots == s ? 9 : tris > 1 ? 9 : 0;

            _brdr[s+y].Toggle(tgl); 
            _brdr[(s*2)+y].Toggle(tgl);
        }

        // Check Columns
        for ( int x = 0; x < s; x++ )
        {
            int tris = 0;
            int dots = 0;
            int reds = 0;

            for ( int y = 0; y < s; y++ )
            {
                if ( _grid[x,y] == 3 ) tris++;
                if ( _grid[x,y] == 4 ) reds++;
                if ( _grid[x,y]  < 0 ) dots++;
            }

            _bordr[x]       = (tris == 1 && reds == 0) ? 1 : 0; 
            _bordr[(s*3)+x] = (tris == 1 && reds == 0) ? 1 : 0;

            int tgl = reds > 0 ? 9 : tris == 1 ? 1 : dots == s ? 9 : tris > 1 ? 9 : 0;

            _brdr[x].Toggle(tgl); 
            _brdr[(s*3)+x].Toggle(tgl);
        }
    }

    private bool IsBoardAllGreen()
    {
        foreach ( int bulb in _bordr ) 
        { if ( bulb != 1 ) return false; }
        return true;
    }

    private void InitGrid()
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
    
    private void InitColors()
    {
        // Select a Random Grid Solution
        ParxManager.instance.solutionNo = UnityEngine.Random.Range(0, ParxManager.gridSolutions.Length);
        string sol = ParxManager.gridSolutions[ParxManager.instance.solutionNo];

        // Randomize Color Order
        List<int> C  = new List<int>();
        List<int> RC = new List<int>();
        for ( int x = 0; x < gridSize; x++ ) C.Add(x + 1);
        for ( int x = 0; x < gridSize; x++ )
        {
            int rc = UnityEngine.Random.Range(0, C.Count);
            RC.Add(C[rc]);
            C.RemoveAt(rc);
        }

        // Set Solution Tile Colors
        for ( int x = 0; x < gridSize; x++ )
        {
            _clrs[x, sol[x]-'0'] = RC[x];
        }

        // Grow Solution Squares
        int N = (gridSize * gridSize) - gridSize;
        for ( int x = 0; x < gridSize; x++ )
        {
            int c = RC[x];
            int y = sol[x] - '0';
            int n = x == gridSize-1 ? N : UnityEngine.Random.Range(1, N/2);
            
            GrowColors(x,y,c,n);

            N -= n;
        }

        // Fix Missed Squares
        int edgeCaseCheck = 0;
        int edgeTimeOutDt = 0;
        do
        {
            for ( int x = 0; x < s; x++ )
            {
                for ( int y = 0; y < s; y++ )
                {
                    if( _clrs[x, y] == 0 )
                    {
                        MatchNeighbourColour(x,y);
                        edgeCaseCheck += _clrs[x, y] == 0 ? 1 : 0;
                    }
                }
            }

            edgeCaseCheck -= edgeTimeOutDt++;
        } 
        while (edgeCaseCheck > 0);
    }

    private void GrowColors(int x, int y, int c, int n)
    {
        List<Tuple<int,int>> neighbours = GetColorlessNeighbours(x,y);
        do
        {
            for ( int i = 0; i < neighbours.Count; i++ )
            {
                int nX = neighbours[i].Item1;
                int nY = neighbours[i].Item2;

                int roll = UnityEngine.Random.Range(0, 100);
                if ( roll > 50 ) { _clrs[nX, nY] = c; n--; }
            }

            if ( n > 0 ) neighbours = GetColorlessNeighbours(x,y);
        } 
        while ( n > 0 && neighbours.Count > 0);
        
    }

    private List<Tuple<int,int>> GetColorlessNeighbours(int x, int y)
    {
        List<Tuple<int,int>> neighbours = new List<Tuple<int, int>>();

        int c = _clrs[x,y];
        _clrs[x,y] *= -1; // To avoid infinite loop -<>- Reset at end

        int[] Xi = {x, x+1, x, x-1};
        int[] Yi = {y-1, y, y+1, y};

        for ( int i = 0; i < 4; i++ )
        {
            if ( Xi[i] < 0         || Yi[i] < 0         ) continue;
            if ( Xi[i] >= gridSize || Yi[i] >= gridSize ) continue;

            if ( _clrs[Xi[i], Yi[i]] == 0 )
            neighbours.Add(new Tuple<int, int>(Xi[i], Yi[i]));

            if ( _clrs[Xi[i], Yi[i]] == c )
            neighbours.AddRange(GetColorlessNeighbours(Xi[i], Yi[i]));
        }

        _clrs[x,y] = c;

        return neighbours;
    }

    private void MatchNeighbourColour(int x, int y)
    {
        int[] Xi = {x, x+1, x, x-1};
        int[] Yi = {y-1, y, y+1, y};

        for ( int i = 0; i < 4; i++ )
        {
            if ( Xi[i] < 0         || Yi[i] < 0         ) continue;
            if ( Xi[i] >= gridSize || Yi[i] >= gridSize ) continue;

            if ( _clrs[Xi[i], Yi[i]] != 0 )
            {
                _clrs[x,y] = _clrs[Xi[i], Yi[i]];
                break;
            }
        }
    }

    // Number of Trees in a Colour Space
    public int CountTreesAtColour(int c)
    {
        int count = 0;

        for ( int x = 0; x < s; x++ )
        {
            for ( int y = 0; y < s; y++ )
            {
                if ( _clrs[x,y] == c && _grid[x,y] >= 3 ) { count++; }
            }
        }

        return count;
    }

    // Detect Trees & Set To RED if they 
    // Touch or Share a Colour Space
    public void ReviewMarkers()
    {
        for ( int x = 0; x < s; x++ )
        {
            for ( int y = 0; y < s; y++ )
            {
                if ( _grid[x, y] >= 3 )
                {
                    bool tt = TouchingTrees(x,y);
                    tt = tt ? true : CountTreesAtColour(_clrs[x,y]) > 1;

                    _grid[x, y] = tt ? 4 : 3;

                    _blox[x,y].ColourMarker(tt ? Color.red : Color.white);
                }
            }
        }
    }

    public void ToggleGlows(int c)
    {
        List<ParxBlock> sameColorBlox = new List<ParxBlock>();

        int tris = 0;
        int reds = 0;

        for ( int x = 0; x < s; x++ )
        {
            for ( int y = 0; y < s; y++ )
            {
                if ( _clrs[x,y] == c ) 
                { 
                    sameColorBlox.Add(_blox[x,y]); 

                    if ( _grid[x,y] == 3) { tris++; }
                    if ( _grid[x,y] == 4) { reds++; }
                }

            }
        }

        bool toggle = tris == 1 && reds == 0;

        foreach ( ParxBlock block in sameColorBlox )
        {
            block.ToggleEmission(toggle);
        }
    }

    // Returns True if ANY Neighbour (Inc. Diagonal) is a tree
    public bool TouchingTrees(int x, int y)
    {
        // 7  0  1
        // 6 x,y 2
        // 5  4  3

        int[] Xi = {x, x+1, x+1, x+1, x, x-1, x-1, x-1};
        int[] Yi = {y-1, y-1, y, y+1, y+1, y+1, y, y-1};

        for ( int i = 0; i < 8; i++ )
        {
            if ( Xi[i] < 0         || Yi[i] < 0         ) continue;
            if ( Xi[i] >= gridSize || Yi[i] >= gridSize ) continue;

            if ( _grid[Xi[i], Yi[i]] >= 3 )
            {
                return true;
            }
        }

        return false;
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
            _brdr[i].Toggle(0);
        }

        UpdateGridPresentation();
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

        InitGrid();
        InitColors();
        InitGridBlox();
        InitGridBorder();
        
        CameraController.instance.FrameGridToCamera();

        // PrintGrid();
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

    public void InitGridBlox()
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
                _blox[x,y].Init(x,y, _clrs[x,y]);
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
        newBlock.name               = "ParxBlock (" + x + " , " + y + ")";

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
    
        bulbRef.Toggle(0);
        return bulbRef;
    }

    private void UpdateGridPresentation()
    {
        for ( int y = 0; y < s; y++ )
        {
            for ( int x = 0; x < s; x++ )
            {
                _blox[x,y].ToggleMarker(_grid[x,y]);
                _blox[x,y].SetText(" ");
            }
        }
    }

}
