using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NameGame : MonoBehaviour
{
    // the main game loop for 'naming' things game, like Elements or Countries

    public static NameGame Manager;

    public TextAsset DataCSV;
    public Button3DElement[] Butts;
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

    public bool UnlockElement(string element)
    {
        ElementData el = allElementsData.FirstOrDefault(e => e.Names.Contains(element.ToLower()));
        
        if ( el != null && !el.Unlocked)
        {
            int index = allElementsData.IndexOf(el);
            Butts[index].SetText_AtomicNumber((index+1).ToString());
            Butts[index].SetText_Symbol(allElementsData[index].Symbol);
            Butts[index].PingTextColor(Color.green, Color.white, 1.3f);
            el.Unlocked = true;
            return true;
        }
        
        return false;
    }

    public bool CheckPrefix(string input)
    {
        string normalized = input.ToLower();

        List<ElementData> alreadyUnlocked = new();

        for (int i = 0; i < allElementsData.Count; i++)
        {
            if (allElementsData[i].Names.Any(name => name.StartsWith(normalized)))
            {
                if (!allElementsData[i].Unlocked) return true;
                else alreadyUnlocked.Add(allElementsData[i]);
            }
        }

        if ( alreadyUnlocked.Count > 0 )
        {
            foreach (ElementData el in alreadyUnlocked)
            {
                int index = allElementsData.IndexOf(el);
                Butts[index].PingTextColor(Color.red, Color.white, 1.5f);
            }
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
        allElementsData = new();

        string[] rows = DataCSV.text.Split('\n');

        for (int i = 1; i < rows.Length; i++) 
        {
            string[] columns = rows[i].Trim().Split(',');

            ElementData data = new ElementData
            {
                Number = int.Parse(columns[0]), 
                Symbol = columns[1],
                Names = new List<string>(){columns[2].ToLower()}
            };

            if (!columns[3].Equals(string.Empty))
            {
                string[] extraNames = columns[3].Split(':');
                for (int x = 0; x < extraNames.Length; x++)
                { data.Names.Add(extraNames[x].ToLower()); }
            }
            
            allElementsData.Add(data);
            Butts[i-1].eData = data;
        }
    }
}

[System.Serializable]
public class ElementData
{
    public int Number;
    public string Symbol;
    public List<string> Names;
    public bool Unlocked = false;
}

// TODO: Add board reset 
