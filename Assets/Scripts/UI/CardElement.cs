using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace UnityRoyale
{
    public class CardElement : VisualElement
    {
        // UxmlFactory and UxmlTraits allow UIBuilder to use CardElement as a building block
        public new class UxmlFactory : UxmlFactory<CardElement, UxmlTraits> {}
        public new class UxmlTraits : VisualElement.UxmlTraits {}

        public CardData cardData { get; private set; }

        public void Init(CardData cardData)
        {
            this.cardData = cardData;
            var portraitImage = this.Q<VisualElement>("image");
            portraitImage.style.backgroundImage = this.cardData.cardImage.texture;
        }
        
        public void ChangeActiveState(bool isActive)
        {
            this.style.opacity = (isActive) ? .05f : 1f;
        }

        public void MoveAndScaleIntoPosition(int cardSlot, Vector2 position)
        {
            AnimatedMoveTo(position, .2f + (.05f * cardSlot));
            Scale(1f);
        }
        
        public void Scale(float ratio)
        {
            transform.scale = Vector3.one * ratio;
        }
        public void AnimatedScale(float endScale, float tweenDuration)
        {
            experimental.animation.Scale(endScale, Mathf.RoundToInt(tweenDuration * 1000)).Ease(Easing.OutQuad);
        }

        public void MoveTo(Vector2 screenPosition)
        {
            transform.position = new Vector3(screenPosition.x, screenPosition.y, transform.position.z);
        }
        public void AnimatedMoveTo(Vector2 endPosition, float tweenDuration)
        {
            experimental.animation.Position(new Vector3(endPosition.x, endPosition.y, transform.position.z),
                Mathf.RoundToInt(tweenDuration * 1000)).Ease(Easing.OutQuad);
        }
        public void Translate(Vector2 screenPositionDelta)
        {
            transform.position += (Vector3) screenPositionDelta;
        }

        public void SetAsLastSibling()
        {
            BringToFront();
        }

        public void Delete()
        {
            RemoveFromHierarchy();
        }
    }
}