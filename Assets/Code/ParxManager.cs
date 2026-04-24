using System.IO;
using UnityEngine;

public class ParxManager : MonoBehaviour
{
    public static ParxManager instance;

    public bool ClearBoard = false;
    public bool RegenerateBoard = false;
    [Space]
    public int solutionNo = 0;
    public bool placeSolution = false;
    [Space]
    public Button3D nextButton;
    public Button3D updtButton;

    [Space]
    public TextAsset gridSolutions5;
    public TextAsset gridSolutions6;
    public TextAsset gridSolutions7;
    public TextAsset gridSolutions8;
    
    public static string[] gridSolutions;
    
    void Awake()
    {
        instance = this;
    }
    
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
            LoadGridSolutions(Parx.instance.gridSize);
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
        #if UNITY_WEBGL
        
            string fileData = string.Empty;

            switch(gridSize)
            {
                case 5: fileData = gridSolutions5.text; break;
                case 6: fileData = gridSolutions6.text; break;
                case 7: fileData = gridSolutions7.text; break;
                case 8: fileData = gridSolutions8.text; break;
            }

            gridSolutions = fileData.Split("\n");

        #else

            string fileName = Application.streamingAssetsPath + "/gridSolutions_" + gridSize + ".txt";
            gridSolutions = File.ReadAllLines(fileName);

        #endif
    }
}
