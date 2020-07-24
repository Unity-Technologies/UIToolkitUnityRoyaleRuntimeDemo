using UnityEngine;
using UnityEngine.UIElements;

namespace UnityRoyale
{
    public class OptionsScreen : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<OptionsScreen, UxmlTraits> { }

        public const string PlayerPrefsMuteMusicKey = "muteMusic";

        private Toggle musicToggle;
        private SliderInt speedSlider;
        private Label speedValue;

        public OptionsScreen()
        {
            this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        void OnGeometryChange(GeometryChangedEvent evt)
        {
            musicToggle = this.Q<Toggle>("music-toggle");
            bool muteMusic = PlayerPrefs.GetInt(PlayerPrefsMuteMusicKey, 0) == 0;
            musicToggle?.SetValueWithoutNotify(!muteMusic);
            SetMuteMusic(muteMusic);
            musicToggle?.RegisterValueChangedCallback(e => OnMusicToggle(e));

            speedSlider = this.Q<SliderInt>("speed-slider");
            speedValue = this.Q<Label>("speed-value-label");
            int timeScale = (int)Mathf.Round(Time.timeScale);

            // Capping (only matters in the Editor where the time scale can be changed on the project properties
            // but we're being safe).
            if (timeScale > 3)
                timeScale = 3;
            else if (timeScale < 1)
                timeScale = 1;

            if (speedValue != null)
            {
                speedValue.text = timeScale.ToString();
                speedSlider.value = timeScale;
                speedSlider.RegisterValueChangedCallback(e => OnSpeedSliderChanged(e.newValue));
            }

            this.UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        private void OnMusicToggle(ChangeEvent<bool> evt)
        {
            bool muteMusic = !evt.newValue;
            SetMuteMusic(muteMusic);
        }

        private void SetMuteMusic(bool muteMusic)
        {
            if (musicToggle == null)
                return;

            if (muteMusic)
            {
                musicToggle.text = "OFF";
                PlayerPrefs.SetInt(PlayerPrefsMuteMusicKey, 0);
            }
            else
            {
                musicToggle.text = "ON";
                PlayerPrefs.SetInt(PlayerPrefsMuteMusicKey, 1);
            }
        }

        private void OnSpeedSliderChanged(int value)
        {
            speedValue.text = value.ToString();
            Time.timeScale = value;
        }
    }
}
