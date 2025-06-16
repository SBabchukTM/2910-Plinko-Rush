using System;
using System.Threading;
using Application.Services.Audio;
using Core.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class SettingsPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;

        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _soundToggle;

        public event Action<float> SoundVolumeChangeEvent;
        public event Action<float> MusicVolumeChangeEvent;

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            SettingsPopupData settingsPopupData = data as SettingsPopupData;

            var soundVolume = settingsPopupData.SoundVolume;
            _soundToggle.isOn = soundVolume > 0f;

            var musicVolume = settingsPopupData.MusicVolume;
            _musicToggle.isOn = musicVolume > 0f;

            _soundToggle.onValueChanged.AddListener(OnSoundVolumeToggleValueChanged);
            _musicToggle.onValueChanged.AddListener(OnMusicVolumeToggleValueChanged);

            _closeButton.onClick.AddListener(DestroyPopup);

            AudioService.PlaySound(ConstAudio.OpenPopupSound);

            return base.Show(data, cancellationToken);
        }

        public override void DestroyPopup()
        {
            AudioService.PlaySound(ConstAudio.PressButtonSound);
            Destroy(gameObject);
        }

        private void OnSoundVolumeToggleValueChanged(bool value)
        {
            var val = value == true ? 1 : 0;

            SoundVolumeChangeEvent?.Invoke(val);
        }

        private void OnMusicVolumeToggleValueChanged(bool value)
        {
            var val = value == true ? 1 : 0;
            MusicVolumeChangeEvent?.Invoke(val);
        }
    }
}