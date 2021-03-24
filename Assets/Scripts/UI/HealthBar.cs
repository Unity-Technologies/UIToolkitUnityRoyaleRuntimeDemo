using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityRoyale
{
    [RequireComponent(typeof(UIDocument))]
    public class HealthBar : MonoBehaviour
    {
        private const string HiddenHealthBarStyleClass = "health-bar--hidden";
        
        // Prefab properties
        [SerializeField] private Vector3 unitAnchorPosition = new Vector3(0f, 3f, 0f);
        [SerializeField] private Vector3 nonUnitAnchorPosition = new Vector3(1.8f, 2f, 0f);
        [SerializeField] private Vector2 worldSize = new Vector2(1.2f, 0.6f);
        [SerializeField] private Color red = new Color32(252, 35, 13, 255);
        [SerializeField] private Color blue = new Color32(31, 132, 255, 255);

        
        [SerializeField] private bool isHidden = true;

        [HideInInspector] public float originalHealth;
        [HideInInspector] public float currentHealth;
        [HideInInspector] public Vector3 anchorPosition;
        [HideInInspector] public Color barColor;
        [HideInInspector] public Transform transformToFollow;

        private VisualElement bar;
        private VisualElement wholeWidget;
        
        public void Initialize(ThinkingPlaceable p)
        {            
            currentHealth = originalHealth = p.hitPoints;
            anchorPosition = p.pType == Placeable.PlaceableType.Unit ? unitAnchorPosition : nonUnitAnchorPosition;
            barColor = p.faction == Placeable.Faction.Player ? red : blue;
            transformToFollow = p.transform;
        }

        void OnEnable()
        {
            var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
            
            // TODO remove this check after the execution order is fixed!
            if (rootVisualElement != null)
            {
                bar = rootVisualElement.Q("Bar");
                wholeWidget = rootVisualElement.Q("HealthBar");
            }
        }

        private void Start()
        {
            // TODO remove this check after the execution order is fixed!
            if (bar == null)
            {
                return;
            }
            bar.style.unityBackgroundImageTintColor = barColor;
            SetHealth(currentHealth);
        }

        public void SetHealth(float newHealth)
        {
            currentHealth = newHealth;
            isHidden = newHealth >= originalHealth;

            float ratio = newHealth > 0f ? newHealth / originalHealth : 1e-5f;
            bar.transform.scale = new Vector3(ratio, 1, 1);
            
            // Hide the health bar after the position is set, otherwise it won't hide.
            wholeWidget.EnableInClassList(HiddenHealthBarStyleClass, isHidden);
        }

        private void Update()
        {
            // TODO remove this check after the execution order is fixed!
            if (bar == null)
            {
                var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
            
                if (rootVisualElement != null)
                {
                    bar = rootVisualElement.Q("Bar");
                    wholeWidget = rootVisualElement.Q("HealthBar");
                    bar.style.unityBackgroundImageTintColor = barColor;
                    SetHealth(currentHealth);
                }
            }
            
            wholeWidget.EnableInClassList(HiddenHealthBarStyleClass, isHidden);
        }

        // Wait for LateUpdate 1) to allow tracked object to move and
        //                     2) to leave time for wholeWidget.layout to be refreshed
        private void LateUpdate()
        {
            // TODO remove this check after the execution order is fixed!
            if (bar == null)
            {
                return;
            }
            if (!isHidden && transformToFollow != null)
            {
                MoveAndScaleToWorldPosition(wholeWidget, transformToFollow.position + anchorPosition, worldSize);
            }
        }

        static void MoveAndScaleToWorldPosition(VisualElement element, Vector3 worldPosition, Vector2 worldSize)
        {
            Rect rect = RuntimePanelUtils.CameraTransformWorldToPanelRect(element.panel, worldPosition, worldSize, Camera.main);
            Vector2 layoutSize = element.layout.size;
            
            // Don't set scale to 0 or a negative number.
            Vector2 scale = layoutSize.x > 0 && layoutSize.y > 0 ? rect.size / layoutSize : Vector2.one * 1e-5f;

            element.transform.position = rect.position;
            element.transform.scale = new Vector3(scale.x, scale.y, 1);
        }
    }
}