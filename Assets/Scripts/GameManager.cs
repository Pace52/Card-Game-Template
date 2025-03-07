using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackjackDealer : MonoBehaviour
{
    [SerializeField] public  List<GameObject> cardPrefabs; // Assign your card prefabs in the inspector
    [SerializeField] public Transform playerHandPosition; // Position where player cards should be dealt
    [SerializeField] public Transform dealerHandPosition; // Position where dealer cards should be dealt
    
    public List<GameObject> deck = new List<GameObject>();
    public List<GameObject> playerHand = new List<GameObject>();
    public List<GameObject> dealerHand = new List<GameObject>();

    // Card display positions
    [SerializeField] private Transform playerCardSpot;
    [SerializeField] private Transform dealerCardSpot;
    private float cardSpacing = 1.5f; // Space between cards

    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;

    [SerializeField] private GameObject cardPrefab; // Single card prefab reference

    public void AddCardToPlayerHand(GameObject card)
    {
        playerHand.Add(card);
        ArrangeCards(playerHand, playerCardSpot);
    }

    public void AddCardToDealerHand(GameObject card, bool faceUp = true)
    {
        dealerHand.Add(card);
        ArrangeCards(dealerHand, dealerCardSpot);
    }

    private void ArrangeCards(List<GameObject> hand, Transform spotTransform)
    {
        for (int i = 0; i < hand.Count; i++)
        {
            GameObject card = hand[i];
            Vector3 newPosition = spotTransform.position + new Vector3(i * cardSpacing, 0, 0);
            
            // Move the card to its new position
            card.transform.position = newPosition;
            card.transform.SetParent(spotTransform);
            
            // Make sure the card is visible and properly oriented
            card.SetActive(true);
            card.transform.rotation = spotTransform.rotation;
            
            // If it's a dealer card, handle face up/down
            if (hand == dealerHand && i == 1) // Second dealer card
            {
                Card cardComponent = card.GetComponent<Card>();
                if (cardComponent != null)
                {
                    cardComponent.Flip(false); // Face down
                }
            }
        }
    }

    public void ClearHands()
    {
        foreach(GameObject card in playerHand)
        {
            Destroy(card);
        }
        foreach(GameObject card in dealerHand)
        {
            Destroy(card);
        }
        playerHand.Clear();
        dealerHand.Clear();
    }

    public void StartGame()
    {
        ClearHands();
        
        // Step 1: Initialize the deck
        InitializeDeck();

        // Step 2: Shuffle the deck
        ShuffleDeck();

        // Step 3: Deal two cards to the player and two cards to the dealer
        DealInitialCards();

        // Output the hands for now
        PrintHands();

        CalculateScore();

        // Add button listeners
        hitButton.onClick.AddListener(PlayerHit);
        standButton.onClick.AddListener(PlayerStand);
    }

    private void InitializeDeck()
    {
        deck.Clear();
        Debug.Log("[INIT] Starting deck initialization...");

        if (cardPrefab == null)
        {
            Debug.LogError("[INIT] ERROR: Card prefab not assigned! Check the Inspector!");
            return;
        }

        // Create 52 cards (13 values × 4 suits)
        for (int value = 1; value <= 13; value++) // 1 = Ace, 11 = Jack, 12 = Queen, 13 = King
        {
            for (int suit = 0; suit < 4; suit++) // 0 = Hearts, 1 = Diamonds, 2 = Clubs, 3 = Spades
            {
                GameObject cardObj = Instantiate(cardPrefab);
                Card cardComponent = cardObj.GetComponent<Card>();
                
                if (cardComponent != null)
                {
                    cardComponent.number = value;
                    cardComponent.suit = suit.ToString();
                    cardObj.name = $"Card_{GetCardName(value)}_{GetSuitName(suit)}";
                    cardObj.SetActive(false); // Hide initially
                    deck.Add(cardObj);
                    Debug.Log($"[INIT] Created {cardObj.name}");
                }
                else
                {
                    Debug.LogError("[INIT] ERROR: Card prefab missing Card component!");
                    Destroy(cardObj);
                }
            }
        }
        
        Debug.Log($"[INIT] Deck initialized with {deck.Count} cards");
    }

    private string GetCardName(int value)
    {
        switch (value)
        {
            case 1: return "Ace";
            case 11: return "Jack";
            case 12: return "Queen";
            case 13: return "King";
            default: return value.ToString();
        }
    }

    private string GetSuitName(int suit)
    {
        switch (suit)
        {
            case 0: return "Hearts";
            case 1: return "Diamonds";
            case 2: return "Clubs";
            case 3: return "Spades";
            default: return "Unknown";
        }
    }

    private void ShuffleDeck()
    {
        Debug.Log("Shuffling deck...");
        System.Random rng = new System.Random();
        
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            GameObject temp = deck[k];
            deck[k] = deck[n];
            deck[n] = temp;
        }

        // Debug output to verify shuffle
        Debug.Log($"Deck shuffled. First few cards:");
        for (int i = 0; i < Mathf.Min(5, deck.Count); i++)
        {
            Card card = deck[i].GetComponent<Card>();
            Debug.Log($"Card {i + 1}: {card.number}");
        }
    }

    private void DealInitialCards()
    {
        if (deck.Count < 4)
        {
            Debug.LogError($"Not enough cards in deck to deal! Deck count: {deck.Count}");
            return;
        }

        Debug.Log("Dealing initial cards...");
        
        // Deal two cards to player
        GameObject playerCard1 = deck[0];
        deck.RemoveAt(0);
        GameObject playerCard2 = deck[0];
        deck.RemoveAt(0);
        
        AddCardToPlayerHand(playerCard1);
        AddCardToPlayerHand(playerCard2);

        // Deal two cards to dealer
        GameObject dealerCard1 = deck[0];
        deck.RemoveAt(0);
        GameObject dealerCard2 = deck[0];
        deck.RemoveAt(0);
        
        AddCardToDealerHand(dealerCard1, true);  // First card face up
        AddCardToDealerHand(dealerCard2, false); // Second card face down

        Debug.Log($"Initial deal complete. Player hand: {playerHand.Count}, Dealer hand: {dealerHand.Count}");
        
        // For debugging
        foreach(GameObject card in playerHand)
        {
            Card cardComponent = card.GetComponent<Card>();
            Debug.Log($"Player has card: {cardComponent.number}");
        }
        
        foreach(GameObject card in dealerHand)
        {
            Card cardComponent = card.GetComponent<Card>();
            Debug.Log($"Dealer has card: {cardComponent.number}");
        }
    }

    // Print the current hands of player and dealer (for debugging)
    private void PrintHands()
    {
        Debug.Log("Player Hand: " + string.Join(", ", playerHand));
        Debug.Log("Dealer Hand: " + string.Join(", ", dealerHand));
    }

    private void CalculateScore()
    {
        Debug.Log("Player Score: " + CalculateHandScore(playerHand));
        Debug.Log("Dealer Score: " + CalculateHandScore(dealerHand));
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

    private void PlayerHit()
    {
        if (deck.Count > 0)
        {
            GameObject newCard = deck[0];
            deck.RemoveAt(0);
            AddCardToPlayerHand(newCard);

            // Check if player busted
            if (CalculateHandScore(playerHand) > 21)
            {
                EndGame("Dealer wins! Player busted!");
                DisableGameButtons();
            }
        }
    }

    private void PlayerStand()
    {
        DisableGameButtons();
        StartCoroutine(DealerTurn());
    }

    private IEnumerator DealerTurn()
    {
        // Dealer hits on 16 or lower
        while (CalculateHandScore(dealerHand) <= 16 && deck.Count > 0)
        {
            yield return new WaitForSeconds(1f); // Add delay for better visualization
            AddCardToDealerHand(deck[0], true);
            deck.RemoveAt(0);
        }

        // Determine winner
        int playerScore = CalculateHandScore(playerHand);
        int dealerScore = CalculateHandScore(dealerHand);

        if (dealerScore > 21)
        {
            EndGame("Player wins! Dealer busted!");
        }
        else if (dealerScore > playerScore)
        {
            EndGame("Dealer wins!");
        }
        else if (playerScore > dealerScore)
        {
            EndGame("Player wins!");
        }
        else
        {
            EndGame("It's a tie!");
        }
    }

    private void DisableGameButtons()
    {
        hitButton.interactable = false;
        standButton.interactable = false;
    }

    private void EnableGameButtons()
    {
        hitButton.interactable = true;
        standButton.interactable = true;
    }

    private void EndGame(string result)
    {
        Debug.Log(result);
        EnableGameButtons();
    }
}
