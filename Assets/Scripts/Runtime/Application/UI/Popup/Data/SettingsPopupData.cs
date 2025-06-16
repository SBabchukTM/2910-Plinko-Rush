using Application.Services.UserData;
using Core.UI;

namespace Application.UI
{
    public class SettingsPopupData : BasePopupData
    {
        private float _soundVolume = 1f;
        private float _musicVolume = 1f;

        public float SoundVolume => _soundVolume;
        public float MusicVolume => _musicVolume;

        public SettingsPopupData(float soundVolume, float musicVolume)
        {
            _soundVolume = soundVolume;
            _musicVolume = musicVolume;
        }
    }
}