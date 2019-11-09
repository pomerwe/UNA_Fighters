using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private MatchController matchController;

    public Character character;
    public GameObject HPBarPrefab;

    public int Layer;

    public GameObject StatsBar;

    // Start is called before the first frame update
    void Start()
    {
        var pos = gameObject.transform.position;
        pos.z = Layer;
        transform.position = pos;
        matchController = GameObject.Find("MatchController").GetComponent<MatchController>();
        matchController.characters.Add(gameObject);
        character = new Character(gameObject.name);
        StatsBar = transform.Find("StatsBar").gameObject;
        StatsBar.transform.Find("HP").transform.localScale = new Vector3((character.HP / 100), 0.8f, 1);
        StatsBar.transform.Find("Stamina").transform.localScale = new Vector3((character.Stamina / 100), 0.2f, 1);
    }

    // Update is called once per frame
    void Update()
    {
        StatsBar.transform.Find("HP").transform.localScale = new Vector3((character.HP/100), 0.8f, 1);
        StatsBar.transform.Find("Stamina").transform.localScale = new Vector3((character.Stamina / 100), 0.2f, 1);
        CheckDeath();

        if(character.Stamina < 100)
        {
            character.Stamina += 0.5f;
            if(character.Stamina >= 100)
            {
                character.Stamina = 100;
                StatsBar.transform.Find("Stamina").GetComponent<SpriteRenderer>().material.color = new Color32(0,255,36,255);
            }
            else
            {
                StatsBar.transform.Find("Stamina").GetComponent<SpriteRenderer>().material.color = new Color32(0, 255, 36, 138);
            }
        }
    }

    private void CheckDeath()
    {
        if(character.HP <= 0)
        {
            matchController.characters.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}

public class Character
{
    public string Name { get; set; }
    public float HP { get; set; }
    public float Stamina { get; set; }
    public int KillCount { get; set; }
    public int DeathCount { get; set; }

    public Character(string name)
    {
        Name = name;
        HP = 100f;
        Stamina = 0f;
        KillCount = 0;
        DeathCount = 0;
    }

    public Character()
    {

    }
}