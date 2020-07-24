using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UIElements;
using System;

namespace UnityRoyale
{
    public class CardManager : MonoBehaviour
    {
        // Prefab properties
        [SerializeField] private Camera mainCamera = default;
        [SerializeField] private LayerMask playingFieldMask = default;
        [SerializeField] private VisualTreeAsset visualTreeCard = default;
        [SerializeField] private UIManager uiManager = default;
        [SerializeField] private DeckData playersDeck = default;
        [SerializeField] private MeshRenderer forbiddenAreaRenderer = default;
		
        public UnityAction<CardData, Vector3, Placeable.Faction> OnCardUsed;
        
        private CardElement[] cards;
        private int cardCount = 3;
        private bool cardIsActive = false; //when true, a card is being dragged over the play field
        private GameObject previewHolder;
        private readonly Vector3 InputCreationOffset = new Vector3(0f, 0f, 1f); //offsets the creation of units so that they are not under the player's finger

        private void Awake()
        {
            previewHolder = new GameObject("PreviewHolder");
            cards = new CardElement[3]; //3 is the length of the dashboard
        }

        public void LoadDeck()
        {
            DeckLoader newDeckLoaderComp = gameObject.AddComponent<DeckLoader>();
            newDeckLoaderComp.OnDeckLoaded += DeckLoaded;
            newDeckLoaderComp.LoadDeck(playersDeck);
        }


        private void DeckLoaded()
        {
            Debug.Log("Player's deck loaded");

            //setup initial cards
            StartCoroutine(AddCardToDeck(.1f));
            for(int i=0; i< cardCount; i++)
            {
                StartCoroutine(PromoteCardFromDeck(i, .4f + i));
                StartCoroutine(AddCardToDeck(.8f + i));
            }
        }

        private VisualElement GetBackupContainer()
        {
            var root = uiManager.GetCardPanelRoot();
            return root.Q<VisualElement>("backup");
        }

        private VisualElement GetActiveContainer()
        {
            var root = uiManager.GetCardPanelRoot();
            return root.Q<VisualElement>("active");
        }

        //moves the preview card from the deck to the active card dashboard
        private IEnumerator PromoteCardFromDeck(int cardId, float delay = 0f)
        {
            yield return new WaitForSecondsRealtime(delay);

            var backupContainer = GetBackupContainer();
            var backupCard = backupContainer.Q<CardElement>();
            
            //setup listeners on Card events
            backupCard.RegisterCallback<MouseDownEvent>(evt => CardTapped(evt, cardId));
            backupCard.RegisterCallback<MouseUpEvent>(evt => CardReleased(evt, cardId));
            backupCard.RegisterCallback<MouseMoveEvent>(evt => CardDragged(evt, cardId));
            
            Vector2 screenPosition = backupCard.LocalToWorld(backupCard.transform.position);

            var activePanel = GetActiveContainer();
            activePanel.Add(backupCard);

            backupCard.MoveTo(backupCard.WorldToLocal(screenPosition));
            backupCard.MoveAndScaleIntoPosition(cardId, ComputeActiveCardPosition(cardId));

            //store a reference to the Card component in the array
            cards[cardId] =  backupCard;
        }

        //adds a new card to the deck on the left, ready to be used
        private IEnumerator AddCardToDeck(float delay = 0f) //TODO: pass in the CardData dynamically
        {
            yield return new WaitForSecondsRealtime(delay);

            //create new card
            var tree = visualTreeCard.CloneTree();
            var card = tree.Q<CardElement>();
            GetBackupContainer().Add(card);

            card.Init(playersDeck.GetNextCardFromDeck());
            card.Scale(0.1f);
            card.AnimatedScale(0.7f, 0.2f);
            card.MoveTo(new Vector2(10, 10));
        }

        private int draggedCardId = -1;
        private Vector2 mouseDownPosition;
        private void CardTapped(MouseDownEvent clickEvent, int cardId)
        {
            cards[cardId].SetAsLastSibling();
			forbiddenAreaRenderer.enabled = true;

            draggedCardId = cardId;
            mouseDownPosition = clickEvent.mousePosition;
        }

        private void CardDragged(IMouseEvent dragEvent, int cardId)
        {
            if (cardId != draggedCardId)
                return;

            var dragAmount = dragEvent.mousePosition - mouseDownPosition;
            cards[cardId].Translate(dragAmount);
            mouseDownPosition = dragEvent.mousePosition;

            //raycasting to check if the card is on the play field
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            bool planeHit = Physics.Raycast(ray, out hit, Mathf.Infinity, playingFieldMask);

            if (planeHit)
            {
                if (!cardIsActive)
                {
                    cardIsActive = true;
                    previewHolder.transform.position = hit.point;
                    cards[cardId].ChangeActiveState(true); //hide card

                    //retrieve arrays from the CardData
                    PlaceableData[] dataToSpawn = cards[cardId].cardData.placeablesData;
                    Vector3[] offsets = cards[cardId].cardData.relativeOffsets;

                    //spawn all the preview Placeables and parent them to the cardPreview
                    for (int i = 0; i < dataToSpawn.Length; i++)
                    {
                        Instantiate(dataToSpawn[i].associatedPrefab, hit.point + offsets[i] + InputCreationOffset,
                            Quaternion.identity, previewHolder.transform);
                    }
                }
                else
                {
                    //temporary copy has been created, we move it along with the cursor
                    previewHolder.transform.position = hit.point;
                }
            }
            else
            {
                if (cardIsActive)
                {
                    cardIsActive = false;
                    cards[cardId].ChangeActiveState(false); //show card

                    ClearPreviewObjects();
                }
            }
        }

        private void CardReleased(MouseUpEvent mouseUpEvent, int cardId)
        {
            //raycasting to check if the card is on the play field
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, playingFieldMask))
            {
                //GameManager registers to OnCardUsed to spawn the actual Placeable
                OnCardUsed?.Invoke(cards[cardId].cardData, hit.point + InputCreationOffset,
                    Placeable.Faction.Player);

                ClearPreviewObjects();
                cards[cardId].Delete(); //remove the card itself

                StartCoroutine(PromoteCardFromDeck(cardId, .2f));
                StartCoroutine(AddCardToDeck(.6f));
            }
            else
            {
                cards[cardId].AnimatedMoveTo(ComputeActiveCardPosition(cardId),.2f);
            }

            forbiddenAreaRenderer.enabled = false;
            draggedCardId = -1;
        }

        private Vector2 ComputeActiveCardPosition(int cardId)
        {
            var activePanel = GetActiveContainer();
            const float margin = 10f;
            float offset = margin + cardId * (activePanel.layout.width - 2*margin) / cardCount;
            return new Vector2(offset, 0);
        }
        
        //happens when the card is put down on the playing field, and while dragging (when moving out of the play field)
        private void ClearPreviewObjects()
        {
            //destroy all the preview Placeables
            for (int i = 0; i < previewHolder.transform.childCount; i++)
            {
                Destroy(previewHolder.transform.GetChild(i).gameObject);
            }
        }
    }

}
