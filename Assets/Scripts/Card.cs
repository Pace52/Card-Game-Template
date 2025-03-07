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
    public int number;
    public int type;
    public string valueString; // "2" through "10", "J", "Q", "K", "A"
    public string suitString; // Optional: "Hearts", "Diamonds", "Clubs", "Spades"
    private bool isFaceUp = true;

    // Start is called before the first frame update
    void Start()
    {
        value = data.value;
        suit = data.suit;
        type = data.type;
        number = data.number;
        sprite = data.sprite;
        //spriteImage.sprite = sprite;
        valueString = value.ToString();
        suitString = suit;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Flip(bool faceUp)
    {
        isFaceUp = faceUp;
        // Add animation or sprite switching logic here
        // For example:
        // spriteRenderer.sprite = faceUp ? frontSprite : backSprite;
    }
}
