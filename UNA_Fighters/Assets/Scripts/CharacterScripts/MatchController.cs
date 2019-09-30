using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchController : MonoBehaviour
{
    public List<GameObject> characters;
    public Text winLabel;
    public GameObject winPanel;

    private void Start()
    {
        characters = new List<GameObject>();
    }
    private void Update()
    {
        CheckCharacters();
        if(characters.Count == 1)
        {
            winLabel.text = (characters[0].name.ToString() + " WINS").ToUpper();
            winPanel.SetActive(true);
        }
    }

    private void CheckCharacters()
    {
        characters.ForEach(c =>{
            if(c == null)
            {
                characters.Remove(c);
            }
        });
    }

}
