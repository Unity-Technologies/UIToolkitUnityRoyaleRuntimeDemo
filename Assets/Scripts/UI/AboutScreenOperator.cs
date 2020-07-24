using UnityEngine;
using UnityEngine.UIElements;

namespace UnityRoyale
{
    public class AboutScreenOperator : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<AboutScreenOperator, UxmlTraits> { }

        ScrollView m_ScrollView;

        public AboutScreenOperator()
        {
            this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        void OnGeometryChange(GeometryChangedEvent evt)
        {
            m_ScrollView = this.Q<ScrollView>();
            Animate();

            this.UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        public void Animate()
        {
            if (m_ScrollView == null)
                return;

            m_ScrollView.transform.position = new Vector2(0, 2000);
            m_ScrollView.experimental.animation.Position(new Vector2(0, 0), 1000);
        }
    }
}
