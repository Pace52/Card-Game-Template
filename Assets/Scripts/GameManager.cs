using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackjackDealer : MonoBehaviour
{
    [SerializeField] private List<GameObject> cardPrefabs; // Assign your card prefabs in the inspector
    [SerializeField] private Transform playerHandPosition; // Position where player cards should be dealt
    [SerializeField] private Transform dealerHandPosition; // Position where dealer cards should be dealt
    
    private List<GameObject> deck = new List<GameObject>();
    private List<GameObject> playerHand = new List<GameObject>();
    private List<GameObject> dealerHand = new List<GameObject>();

    // Spacing between cards
    private float cardOffset = 1.5f;

    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;

    public void AddCardToPlayerHand(GameObject card)
    {
        int cardIndex = playerHand.Count;
        card.SetActive(true);
        card.transform.position = playerHandPosition.position + new Vector3(cardIndex * cardOffset, 0, 0);
        card.transform.SetParent(playerHandPosition);
        Card cardComponent = card.GetComponent<Card>();
        cardComponent.Flip(true); // Player cards always face up
        playerHand.Add(card);
        UpdateHandDisplay(playerHand, playerHandPosition);
    }

    public void AddCardToDealerHand(GameObject card, bool faceUp = true) 
    {
        int cardIndex = dealerHand.Count;
        card.SetActive(true);
        card.transform.position = dealerHandPosition.position + new Vector3(cardIndex * cardOffset, 0, 0);
        card.transform.SetParent(dealerHandPosition);
        Card cardComponent = card.GetComponent<Card>();
        cardComponent.Flip(faceUp);
        dealerHand.Add(card);
        UpdateHandDisplay(dealerHand, dealerHandPosition);
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
        Debug.Log($"Initializing deck with {cardPrefabs.Count} card prefabs");
        if (cardPrefabs == null || cardPrefabs.Count == 0)
        {
            Debug.LogError("No card prefabs assigned!");
            return;
        }

        foreach (GameObject cardPrefab in cardPrefabs)
        {
            for (int i = 0; i < 4; i++) // 4 suits
            {
                GameObject card = Instantiate(cardPrefab);
                card.SetActive(false); // Hide cards initially
                deck.Add(card);
            }
        }
        Debug.Log($"Deck initialized with {deck.Count} cards");
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
        if (deck.Count < 4)
        {
            Debug.LogError($"Not enough cards in deck to deal! Deck count: {deck.Count}");
            return;
        }

        Debug.Log("Dealing initial cards...");
        // Deal two cards to player
        AddCardToPlayerHand(deck[0]);
        deck.RemoveAt(0);
        AddCardToPlayerHand(deck[0]);
        deck.RemoveAt(0);

        // Deal two cards to dealer - first face up, second face down
        AddCardToDealerHand(deck[0], true);
        deck.RemoveAt(0);
        AddCardToDealerHand(deck[0], false);
        deck.RemoveAt(0);
        Debug.Log($"Initial deal complete. Player hand: {playerHand.Count}, Dealer hand: {dealerHand.Count}");
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
            AddCardToPlayerHand(deck[0]);
            deck.RemoveAt(0);

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

    private void UpdateHandDisplay(List<GameObject> hand, Transform position)
    {
        // Implementation of UpdateHandDisplay method
    }
}
