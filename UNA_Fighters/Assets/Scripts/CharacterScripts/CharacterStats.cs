using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Character character;
    public GameObject HPBarPrefab;

    private GameObject HPBar;

    // Start is called before the first frame update
    void Start()
    {
        character = new Character(gameObject.name);
        HPBar = transform.Find("HPBar").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        HPBar.transform.Find("HP").transform.localScale = new Vector3((character.HP/100), 1, 1);
        CheckDeath();
    }

    private void CheckDeath()
    {
        if(character.HP < 0)
        {
            Destroy(gameObject);
        }
    }
}

public class Character
{
    public string Name { get; set; }
    public float HP { get; set; }
    public float MP { get; set; }
    public int KillCount { get; set; }
    public int DeathCount { get; set; }

    public Character(string name)
    {
        Name = name;
        HP = 100f;
        MP = 0f;
        KillCount = 0;
        DeathCount = 0;
    }

    public Character()
    {

    }
}