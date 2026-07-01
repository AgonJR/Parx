using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NameGame : MonoBehaviour
{
    // the main game loop for 'naming' things game, like Elements or Countries

    public static NameGame Manager;

    public TextAsset DataCSV;
    public TextAsset HintsCSV;
    public Button3DElement[] Butts;
    public List<ElementData> allElementsData;
    public int TotalUnlocked => allElementsData.Count(el => el.Unlocked);

    [Space]

    public GameObject PauseBlocker;
    public Animator pauseAnimator;
    public bool Paused = false;

    [Space]

    public TextMesh HintTextbox1;
    public TextMesh HintTextbox2;
    public Animator hintAnimator;

    [Space]

    public MeshRenderer musicUIToggle;
    public MeshRenderer sfxUIToggle;
    public TextMesh ProgeressCount;
    public TextDetector TextComp;
    public Material matToggleOff;
    public Material matToggleOn;
    public AudioSource asMusic;

    [Space]

    public bool HintDisplayed = false;
    public int  HintedElement = 0;
    public bool Music = true;
    public bool SFX = true;

    private void Awake()
    {
        Manager = this;
    }

    private void Start()
    {
        CollectButts();
        ReadElements();
        ReadHints();
        LoadSave();
        HideHint();
    }

    public bool UnlockElement(string element, bool saveGame = true)
    {
        ElementData el = allElementsData.FirstOrDefault(e => e.Names.Contains(element.ToLower()));
        
        if ( el != null && !el.Unlocked)
        {
            el.Unlocked = true;
            int index = allElementsData.IndexOf(el);
            if (HintedElement == el.Number) { HideHint(); }
            Butts[index].SetText_AtomicNumber((index+1).ToString());
            Butts[index].SetText_Symbol(allElementsData[index].Symbol);
            Butts[index].PingTextColor(Color.green, Color.white, 1.3f);
            Butts[index].UpdateMaterial();
            if(saveGame) SaveGame();
            return true;
        }

        
        return false;
    }

    public void UnlockAll()
    {
        foreach (ElementData el in allElementsData)
        {
            UnlockElement(el.Names.FirstOrDefault(), false);
        }
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
                Butts[index].PingTextColor(Color.orange, Color.white, 1.5f);
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

    private void ReadElements()
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

    private void ReadHints()
    {
        string[] rows = HintsCSV.text.Split('\n');

        for (int i = 0; i < (rows.Length / 4); i++) 
        {
            int lastIndex = 0;
            int offset = i * 4;
            string[] meta = rows[offset].Split(',');
            int elementIndex = int.Parse(meta[0]) - 1;
            allElementsData[elementIndex].Hints = new List<string>();

            for (int t = 1; t < meta.Length; t++ )
            {
                int index = int.Parse(meta[t]); if (index == 0) continue;

                      if (index == lastIndex)  
                      { allElementsData[elementIndex].Hints[index-1] += " \n"; }
                else  { allElementsData[elementIndex].Hints.Add(string.Empty); }

                if (index > 0) { allElementsData[elementIndex].Hints[index-1] += rows[offset+t]; }

                lastIndex = index;
            }
        }
    }

    public void Reset()
    {
        foreach (ElementData el in allElementsData)
        {
            el.Unlocked = false;
        }

        for (int i = 0; i < Butts.Length; i++)
        {
            Butts[i].ResetZoom(); 
            Butts[i].UpdateMaterial(); 
            Butts[i].SetText_Name(string.Empty);
            Butts[i].SetText_Symbol(string.Empty);
            Butts[i].SetText_AtomicNumber(string.Empty);
        }

        TextComp.ResetInput();
    }

    public static void ShowHint(string hint, int element)
    {
        if(!Manager.HintDisplayed) 
            Manager.hintAnimator.Play("HintEnter");

        Manager.HintTextbox1.text = hint;
        Manager.HintTextbox2.text = hint;
        Manager.HintedElement = element;
        Manager.HintDisplayed = true;
    }

    public static void HideHint()
    {
        if(Manager.HintDisplayed)
        {
            Manager.hintAnimator.Play("HintExit");
            Manager.HintDisplayed = false;
        }
    }

    public static void PauseGame(bool pause)
    {
        Manager.Paused = pause;
        Manager.PauseBlocker.SetActive(pause);
        Manager.pauseAnimator.Play(pause ? "PauseMenuIn" : "PauseMenuOut");
        if (pause) Manager.ProgeressCount.text = Manager.TotalUnlocked.ToString() + " / 118";
        if (pause) HideHint();
    }

    public static void ToggleMusic()
    {
        Manager.Music = !Manager.Music;
        Manager.asMusic.enabled = Manager.Music;
        Manager.musicUIToggle.material = Manager.Music ? Manager.matToggleOn : Manager.matToggleOff;
        PlayerPrefs.SetInt("EleNamesMusic", Manager.Music ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void ToggleSFX()
    {
        Manager.SFX = !Manager.SFX;
        Manager.TextComp.EnableSFX(Manager.SFX);
        Manager.sfxUIToggle.material = Manager.SFX ? Manager.matToggleOn : Manager.matToggleOff;
        PlayerPrefs.SetInt("EleNamesSFX", Manager.SFX ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void ExitGame()
    {
        Application.Quit();
    }

    public void SaveGame()
    {
        // Aggregate converts unlock booleans into a single "10110..." string
        string data = allElementsData.Aggregate(new StringBuilder(), (sb, element) => sb.Append(element.Unlocked ? '1' : '0')).ToString();
        
        PlayerPrefs.SetString("UnlockedElements", data);
        PlayerPrefs.Save(); // Needed for WebGL support
    }

    public void ClearSave()
    {
        PlayerPrefs.SetString("UnlockedElements", new string('0', allElementsData.Count));
        PlayerPrefs.Save(); // Creates a string of '0's
    }

    public void LoadSave()
    {
        string unlockData = PlayerPrefs.GetString("UnlockedElements", "");

        if (unlockData.Length == allElementsData.Count)
        {
            for (int i = 0; i < allElementsData.Count; i++)
            {
                if (unlockData[i] == '1')
                    UnlockElement(allElementsData[i].Names.FirstOrDefault(), false);
            }
        }

        if ( PlayerPrefs.GetInt("EleNamesSFX", 1) == 0 ) ToggleSFX();
        if ( PlayerPrefs.GetInt("EleNamesMusic", 1) == 0 ) ToggleMusic();
    }
}

[System.Serializable]
public class ElementData
{
    public int Number;
    public string Symbol;
    public List<string> Names;
    public List<string> Hints;
    public int HintsIndex = 0;
    public bool Unlocked = false;
    public int DisplayNameIndex = 0;
    public string DisplayName => Names[DisplayNameIndex];
}
