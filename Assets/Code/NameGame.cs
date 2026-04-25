using System.Collections.Generic;
using UnityEngine;

public class NameGame : MonoBehaviour
{
    // the main game loop for 'naming' things game, like Elements or Countries

    public static NameGame Manager;

    public List<string> _lockedElements;

    private void Start()
    {
        _lockedElements = new List<string>()
        {
            "test",
            "apple",
            "banana",
            "kikikikikikikiki",
        };

        Manager = this;
    }

    private void Update()
    {
        
    }

    public bool UnlockElement(string element)
    {
        if (_lockedElements.Contains(element))
        {
            _lockedElements.Remove(element);
            return true;
        }
        
        return false;
    }

    public bool CheckPrefix(string input)
    {
        for (int i = 0; i < _lockedElements.Count; i++)
        {
            if (_lockedElements[i].StartsWith(input))
                return true;
        }
        
        return false;
    }
}
