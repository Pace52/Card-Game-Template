using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public List<Card> deck = new List<Card>();
    public List<Card> player_deck = new List<Card>();
    public List<Card> ai_deck = new List<Card>();
    public List<Card> player_hand = new List<Card>();
    public List<Card> ai_hand = new List<Card>();
    public List<Card> discard_pile = new List<Card>();

    public object RandomElement { get; private set; }

    private void Awake()
    {
        if (gm != null && gm != this)
        {
            Destroy(gameObject);
        }
        else
        {
            gm = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Deal()
    {

    }

    void Shuffle()
    {
        
    }

    void AI_Turn()
    {

    }



    
}
*/

public class BlackjackDealer : MonoBehaviour
{
    // Card ranks (used for simplicity, normally you can add the full card set)
    private List<string> deck = new List<string>();

    // Player and Dealer hands
    private List<string> Player_hand = new List<string>();
    private List<string> Ai_hand = new List<string>();

    // To visualize the cards, we can also create UI elements, but this is just logic
    public void StartGame()
    {
        // Step 1: Initialize the deck
        InitializeDeck();

        // Step 2: Shuffle the deck
        ShuffleDeck();

        // Step 3: Deal two cards to the player and two cards to the dealer
        DealInitialCards();

        // Output the hands for now
        PrintHands();
    }

    // Initializes a deck with some simplified card values (normally, it includes suits as well)
    private void InitializeDeck()
    {
        deck.Clear();
        // Add 4 suits of each value (simplified here as "2", "3", ... , "Ace")
        string[] cardValues = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };
        foreach (var value in cardValues)
        {
            for (int i = 0; i < 4; i++) // 4 suits
            {
                deck.Add(value);
            }
        }
    }

    // Shuffle the deck using the Fisher-Yates algorithm
    private void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i);
            // Swap positions
            string temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    // Deal two cards to the player and two to the dealer
    private void DealInitialCards()
    {
        Player_hand.Clear();
        Ai_hand.Clear();

        // Deal two cards to player
        Player_hand.Add(deck[0]);
        Player_hand.Add(deck[1]);
        deck.RemoveAt(0);
        deck.RemoveAt(0);

        // Deal two cards to dealer
        Ai_hand.Add(deck[0]);
        Ai_hand.Add(deck[1]);
        deck.RemoveAt(0);
        deck.RemoveAt(0);
    }

    // Print the current hands of player and dealer (for debugging)
    private void PrintHands()
    {
        Debug.Log("Player Hand: " + string.Join(", ", Player_hand));
        Debug.Log("Dealer Hand: " + string.Join(", ", Ai_hand));
    }
}
