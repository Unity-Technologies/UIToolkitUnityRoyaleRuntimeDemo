using UnityEngine;
using UnityEngine.UIElements;

namespace UnityRoyale
{
    [RequireComponent(typeof(UIDocument))]
    public class GameScreen : MonoBehaviour
    {
        private VisualElement cardPanel;

        void Start()
        {
            cardPanel = GetComponent<UIDocument>().rootVisualElement.Q("cardpanel");
            cardPanel.style.display = DisplayStyle.None;
        }

        public VisualElement GetCardPanelRoot()
        {
            // Enable the screen with the first access to the panel, as it means we want to show cards.
            ShowGameScreen();

            return cardPanel;
        }

        public void HideGameScreen()
        {
            cardPanel.style.display = DisplayStyle.None;
        }

        private void ShowGameScreen()
        {
            cardPanel.style.display = DisplayStyle.Flex;
        }
    }
}
