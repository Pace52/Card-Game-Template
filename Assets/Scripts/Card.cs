using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public partial class Card : MonoBehaviour
{
    public Card_data data;
    public Sprite sprite;
    public int value;
    public string suit;
    public int type;


    // Start is called before the first frame update
    void Start()
    {
        value = data.value;
        suit = data.suit;
        type = data.type;
        sprite = data.sprite;
        //spriteImage.sprite = sprite;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
