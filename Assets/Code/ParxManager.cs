using System.IO;
using UnityEngine;

public class ParxManager : MonoBehaviour
{
    public bool ClearBoard = false;
    public bool RegenerateBoard = false;
    [Space]
    public int solutionNo = 0;
    public bool placeSolution = false;
    [Space]
    public string[] gridSolutions;
    
    
    void Start()
    {
        LoadGridSolutions(Parx.instance.gridSize);
    }

    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.R) || ClearBoard)
        {
            Parx.instance.ClearGrid();
            ClearBoard = false;
        }

        if ( Input.GetKeyDown(KeyCode.N) || RegenerateBoard)
        {
            Parx.instance.RegenerateGrid();
            RegenerateBoard = false;
        }

        if ( placeSolution )
        {
            Parx.instance.ClearGrid();

            int x = 0;
            foreach ( char c in gridSolutions[solutionNo] )
            {
                if ( x >= Parx.instance.gridSize - 1 ) break;
                Parx.instance.PlaceTree(x++, c - '0', 3);
            }

            placeSolution = false;
        }
    }

    private void LoadGridSolutions(int gridSize)
    {
        string fileName = "gridSolutions_" + gridSize + ".txt";
        gridSolutions = File.ReadAllLines(fileName);
    }
}
