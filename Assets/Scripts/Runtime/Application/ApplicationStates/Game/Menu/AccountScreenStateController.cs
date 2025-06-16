using System.Threading;
using Application.UI;
using Core.StateMachine;
using Cysharp.Threading.Tasks;
using Runtime.Application.UserAccountSystem;
using UnityEngine;
using ILogger = Core.ILogger;

namespace Application.Game
{
    public class AccountScreenStateController : StateController
    {
        private readonly IUiService _uiService;
        private readonly UserAccountService _userAccountService;
        private readonly AvatarSelectionService _avatarSelectionService;
        private readonly UserDataValidationService _userDataValidationService;

        private AccountScreen _screen;

        private UserAccountData _modifiedData;

        public AccountScreenStateController(ILogger logger, IUiService uiService,
            UserAccountService userAccountService,
            AvatarSelectionService avatarSelectionService,
            UserDataValidationService userDataValidationService) : base(logger)
        {
            _uiService = uiService;
            _userAccountService = userAccountService;
            _avatarSelectionService = avatarSelectionService;
            _userDataValidationService = userDataValidationService;
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            CopyData();
            CreateScreen();
            SubscribeToEvents();
            return UniTask.CompletedTask;
        }

        public override async UniTask Exit()
        {
            await _uiService.HideScreen(ConstScreens.AccountScreen);
        }

        private void CopyData() => _modifiedData = _userAccountService.GetAccountDataCopy();

        private void CreateScreen()
        {
            _screen = _uiService.GetScreen<AccountScreen>(ConstScreens.AccountScreen);
            _screen.Initialize();
            _screen.ShowAsync().Forget();
            _screen.SetData(_modifiedData);
            _screen.SetAvatar(_userAccountService.GetUsedAvatarSprite(false));
        }

        private void SubscribeToEvents()
        {
            _screen.OnBackPressed += GoToMenu;
            _screen.OnSavePressed += SaveAndLeave;
            _screen.OnChangeAvatarPressed += SelectNewAvatar;
            _screen.OnNameChanged += ValidateName;
        }

        private async void GoToMenu() => await GoTo<MenuStateController>();

        private void SaveAndLeave()
        {
            _userAccountService.SaveAccountData(_modifiedData);
            GoToMenu();
        }

        private async void SelectNewAvatar()
        {
            Sprite newAvatar = await _avatarSelectionService.PickImage(512, CancellationToken.None);

            if (newAvatar != null)
            {
                Texture2D originalTexture = newAvatar.texture;
                Texture2D readableTexture = CreateReadableTexture(originalTexture);
                Texture2D cropped = CropToSquare(readableTexture);

                Sprite squareSprite = Sprite.Create(
                    cropped,
                    new Rect(0, 0, cropped.width, cropped.height),
                    new Vector2(0.5f, 0.5f)
                );

                _modifiedData.AvatarBase64 = _userAccountService.ConvertToBase64(squareSprite);

                _screen.SetAvatar(squareSprite);
            }
        }

        private Texture2D CropToSquare(Texture2D source)
        {
            int size = Mathf.Min(source.width, source.height);
            int startX = (source.width - size) / 2;
            int startY = (source.height - size) / 2;

            Color[] pixels = source.GetPixels(startX, startY, size, size);
            Texture2D croppedTexture = new Texture2D(size, size);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            return croppedTexture;
        }

        private Texture2D CreateReadableTexture(Texture2D original)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(
                original.width,
                original.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(original, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;

            Texture2D readableTexture = new Texture2D(original.width, original.height);
            readableTexture.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            readableTexture.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            return readableTexture;
        }

        private void ValidateName(string value)
        {
            if (!_userDataValidationService.IsNameValid(value))
                _screen.SetData(_modifiedData);
            else
                _modifiedData.Username = value;
        }
    }
}