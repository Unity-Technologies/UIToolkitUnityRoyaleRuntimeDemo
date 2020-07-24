using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace UnityRoyale
{
    [RequireComponent(typeof(UIDocument))]
    public class EndScreen : MonoBehaviour
    {
        private const int AnimationDurationMs = 1000;
        
        [SerializeField] private string winnerTeamName;
        
        private Label matchOverLabel;
        private Label winnerIsLabel;
        private Label winnerTeamNameLabel;
        private Button mainMenuButton;

        private float matchOverLabelPosition;
        private float winnerIsLabelPosition;
        private float winnerTeamNameLabelPosition;

        public static void Show(EndScreen prefab, Transform parent, string winnerTeamName)
        {
            Instantiate(prefab, parent).winnerTeamName = winnerTeamName;
        }

        void Start()
        {
            // Retrieving interesting elements to:
            // 1- set values
            // 2- assign behaviors
            // 3- animate!
            var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

            matchOverLabel = rootVisualElement.Q<Label>("match-over-label");

            winnerIsLabel = rootVisualElement.Q<Label>("winner-is-label");

            winnerTeamNameLabel = rootVisualElement.Q<Label>("winner-label");
            winnerTeamNameLabel.text = winnerTeamName + "!!";

            mainMenuButton = rootVisualElement.Q<Button>("main-menu-button");

            // Attaching callback to the button.
            mainMenuButton.RegisterCallback<ClickEvent>(ev => OnMainMenuButton());

            DoAnimations();
        }

        private void OnMainMenuButton()
        {
            SceneManager.LoadScene("TitleScreen");
        }

        private void DoAnimations()
        {
            // Retrieve positions of the elements we want to move.
            matchOverLabelPosition = matchOverLabel.style.top.value.value;
            winnerIsLabelPosition = winnerIsLabel.style.top.value.value;
            winnerTeamNameLabelPosition = winnerTeamNameLabel.style.top.value.value;
            
            // Make elements disappear so we can make them appear with animation.
            matchOverLabel.style.opacity = 0;
            winnerIsLabel.style.opacity = 0;
            winnerTeamNameLabel.style.opacity = 0;
            mainMenuButton.style.opacity = 0;
            
            // Calling this method starts a chain that will animate all the elements we need to animate.
            DoAnimateMatchOverLabel();
        }

        private void DoAnimateMatchOverLabel()
        {
            MoveDownAndFadeIn(matchOverLabel, matchOverLabelPosition - 100,
                matchOverLabelPosition, AnimationDurationMs, DoAnimateWinnerIsLabel);
        }

        private void DoAnimateWinnerIsLabel()
        {
            MoveDownAndFadeIn(winnerIsLabel, matchOverLabelPosition - 100,
                winnerIsLabelPosition, AnimationDurationMs, DoAnimateWinnerTeamNameLabel);
        }

        private void DoAnimateWinnerTeamNameLabel()
        {
            MoveDownAndFadeIn(winnerTeamNameLabel, matchOverLabelPosition - 100,
                winnerTeamNameLabelPosition, AnimationDurationMs, DoAnimateMainMenuButton);
        }

        private void DoAnimateMainMenuButton()
        {
            FadeIn(mainMenuButton, AnimationDurationMs);
        }
        
        private void MoveDownAndFadeIn(VisualElement elementToAnimate, float initialTopPosition,
            float finalTopPosition, int durationMs, Action callback = null)
        {
            elementToAnimate.experimental.animation
                .Start(new StyleValues {top = initialTopPosition, opacity = 0},
                    new StyleValues {top = finalTopPosition, opacity = 1},
                    durationMs).Ease(Easing.OutQuad).OnCompleted(callback);
        }

        private void FadeIn(VisualElement elementToFadeIn, int durationMs, Action callback = null)
        {
            elementToFadeIn.experimental.animation
                .Start(
                    new StyleValues {opacity = 0},
                    new StyleValues {opacity = 1},
                    durationMs).Ease(Easing.OutQuad).OnCompleted(callback);
        }
    }
}