using UnityEngine;

public class ParxManager : MonoBehaviour
{
    public bool ClearBoard = false;
    public bool RegenerateBoard = false;
    
    void Start()
    {
        
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

    }
}
