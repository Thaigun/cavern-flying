using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRow : MonoBehaviour
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _killsText;
    [SerializeField] private Text _deathsText;

    private string _name;
    private int _kills;
    private int _deaths;

    public string Name
    {
        get { return _name; }
        private set
        {
            _name = value;
            _nameText.text = value;
        }
    }

    public int Kills
    {
        get { return _kills; }
        private set
        {
            _kills = value;
            _killsText.text = value.ToString();
        }
    }

    public int Deaths
    {
        get { return _deaths; }
        private set
        {
            _deaths = value;
            _deathsText.text = value.ToString();
        }
    }

    public void SetData(string name, int kills, int deaths)
    {
        Name = name;
        Kills = kills;
        Deaths = deaths;
    }
}
