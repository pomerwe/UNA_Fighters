using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchController : MonoBehaviour
{
    public List<GameObject> characters = new List<GameObject>();
    public Text winLabel;
    public GameObject winPanel;
    public bool matchEnded = false;

    private void Start()
    {
        matchEnded = false;
    }
    private void Update()
    {
        
        CheckCharacters();
        if(matchEnded)
        {
            winLabel.text = (characters[0].name.ToString() + " WINS").ToUpper();
            winPanel.SetActive(true);
            Invoke("ResetScene", 4f);
            
           
           
          
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void CheckCharacters()
    {
        characters.ForEach(c =>{
            if(c == null)
            {
                characters.Remove(c);
            }
        });


        if (characters.Count == 1)
        {
            matchEnded = true;
        }
    }

}
