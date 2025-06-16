using System.Threading;
using Application.Services.UserData;
using Application.UI;
using Core;
using Core.Services.Audio;
using Cysharp.Threading.Tasks;

namespace Runtime.Application.ApplicationStates.Game.Controllers
{
    public sealed class StartSettingsController : BaseController
    {
        private readonly IUiService _uiService;
        private readonly UserDataService _userDataService;
        private readonly IAudioService _audioService;

        public StartSettingsController(IUiService uiService, UserDataService userDataService, IAudioService audioService)
        {
            _uiService = uiService;
            _userDataService = userDataService;
            _audioService = audioService;
        }

        public override UniTask Run(CancellationToken cancellationToken)
        {
            base.Run(cancellationToken);

            SettingsPopup settingsPopup = _uiService.GetPopup<SettingsPopup>(ConstPopups.SettingsPopup);

            settingsPopup.SoundVolumeChangeEvent += OnChangeSoundVolume;
            settingsPopup.MusicVolumeChangeEvent += OnChangeMusicVolume;

            var userData = _userDataService.GetUserData();

            var soundVolume = userData.SettingsData.SoundVolume;
            var musicVolume = userData.SettingsData.MusicVolume;

            settingsPopup.Show(new SettingsPopupData(soundVolume, musicVolume), cancellationToken).Forget();
            CurrentState = ControllerState.Complete;
            return UniTask.CompletedTask;
        }
        
        
        private void OnChangeSoundVolume(float volume)
        {
            _audioService.SetVolume(AudioType.Sound, volume);
            var userData = _userDataService.GetUserData();
            userData.SettingsData.SoundVolume = volume;
        }

        private void OnChangeMusicVolume(float volume)
        {
            _audioService.SetVolume(AudioType.Music, volume);
            var userData = _userDataService.GetUserData();
            userData.SettingsData.MusicVolume = volume;
        }
    }
}