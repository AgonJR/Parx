using System.Collections.Generic;
using UnityEngine;

public class NameGame : MonoBehaviour
{
    // the main game loop for 'naming' things game, like Elements or Countries

    public static NameGame Manager;

    public TextAsset DataCSV;
    public Button3DElement[] Butts;

    public List<string> _lockedElements;
    public List<ElementData> allElementsData;

    private void Awake()
    {
        Manager = this;
    }

    private void Start()
    {
        CollectButts();
        ReadData();
    }

    private void Update()
    {
        
    }

    public bool UnlockElement(string element)
    {
        string normalized = element.ToLower();

        if (_lockedElements.Contains(normalized))
        {
            int index = _lockedElements.IndexOf(normalized);

            Butts[index].SetText_Symbol(allElementsData[index].Symbol);
            Butts[index].SetText_AtomicNumber((index+1).ToString());
            
            _lockedElements[index] = "Unlocked:" + _lockedElements[index];

            return true;
        }
        
        return false;
    }

    public bool CheckPrefix(string input)
    {
        string normalized = input.ToLower();

        for (int i = 0; i < _lockedElements.Count; i++)
        {
            if (_lockedElements[i].StartsWith(normalized))
                return true;
        }
        
        return false;
    }

    private void CollectButts()
    {
        Butts = GetComponentsInChildren<Button3DElement>();

        for (int i = 0; i < Butts.Length; i++)
        {
            Butts[i].SetText_Symbol(string.Empty);
            Butts[i].SetText_AtomicNumber(string.Empty);
        }
    }

    private void ReadData()
    {
        _lockedElements = new();
        allElementsData = new();

        string[] rows = DataCSV.text.Split('\n');

        for (int i = 1; i < rows.Length; i++) 
        {
            string[] columns = rows[i].Trim().Split(',');

            ElementData data = new ElementData
            {
                Number = int.Parse(columns[0]), 
                Symbol = columns[1],
                Name = columns[2].ToLower()
            };

            allElementsData.Add(data);

            _lockedElements.Add(data.Name);
        }
    }
}

public struct ElementData
{
    public int Number;
    public string Symbol;
    public string Name;
}
