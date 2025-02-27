using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackjackDealer : MonoBehaviour
{
    [SerializeField] private List<GameObject> cardPrefabs; // Assign your card prefabs in the inspector
    [SerializeField] private Transform playerHandPosition; // Position where player cards should be dealt
    [SerializeField] private Transform dealerHandPosition; // Position where dealer cards should be dealt
    
    private List<GameObject> deck = new List<GameObject>();
    private List<GameObject> Player_hand = new List<GameObject>();
    private List<GameObject> Ai_hand = new List<GameObject>();

    // Spacing between cards
    private float cardOffset = 1.5f;

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

        CalculateScore();
    }

    private void InitializeDeck()
    {
        deck.Clear();
        foreach (GameObject cardPrefab in cardPrefabs)
        {
            for (int i = 0; i < 4; i++) // 4 suits
            {
                GameObject card = Instantiate(cardPrefab);
                card.SetActive(false); // Hide cards initially
                deck.Add(card);
            }
        }
    }

    private void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i);
            // Swap positions
            GameObject temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    private void DealInitialCards()
    {
        Player_hand.Clear();
        Ai_hand.Clear();

        // Deal two cards to player
        DealCard(Player_hand, playerHandPosition, 0);
        DealCard(Player_hand, playerHandPosition, 1);

        // Deal two cards to dealer
        DealCard(Ai_hand, dealerHandPosition, 0);
        DealCard(Ai_hand, dealerHandPosition, 1);
    }

    private void DealCard(List<GameObject> hand, Transform position, int cardIndex)
    {
        GameObject card = deck[0];
        deck.RemoveAt(0);
        
        card.SetActive(true);
        card.transform.position = position.position + new Vector3(cardIndex * cardOffset, 0, 0);
        hand.Add(card);
    }

    // Print the current hands of player and dealer (for debugging)
    private void PrintHands()
    {
        Debug.Log("Player Hand: " + string.Join(", ", Player_hand));
        Debug.Log("Dealer Hand: " + string.Join(", ", Ai_hand));
    }

    private void CalculateScore()
    {
        Debug.Log("Player Score: " + CalculateHandScore(Player_hand));
        Debug.Log("Dealer Score: " + CalculateHandScore(Ai_hand));
    }

    private int CalculateHandScore(List<GameObject> hand)
    {
        int score = 0;
        int aceCount = 0;

        foreach (GameObject cardObj in hand)
        {
            Card card = cardObj.GetComponent<Card>();
            if (card.number == 1)
            {
                aceCount++;
                score += 11;
            }
            else if (card.number == 13 || card.number == 12 || card.number == 11)
            {
                score += 10;
            }
            else
            {
                score += (int)card.number;
            }
        }

        // Adjust for aces if score is over 21
        while (score > 21 && aceCount > 0)
        {
            score -= 10;
            aceCount--;
        }

        return score;
    }
}
