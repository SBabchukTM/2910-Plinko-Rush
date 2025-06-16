using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.Application.ShopSystem
{
    public class ShopItemDisplayView : MonoBehaviour
    {
        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private GameObject _purchaseParent;
        [SerializeField] private GameObject _statusParent;
        [SerializeField] private Button _purchaseButton;
        // [SerializeField] private Transform _container;

        private ShopItemDisplayModel _shopItemDisplayModel;

        public event Action<ShopItemDisplayView> OnPurchasePressed;

        [Inject]
        public void Construct(ShopItemDisplayModel shopItemDisplayModel) =>
                _shopItemDisplayModel = shopItemDisplayModel;

        private void OnDestroy()
        {
            _purchaseButton.onClick.RemoveAllListeners();

            if (_shopItemDisplayModel.ShakeTaskCompletionSource.Task.IsCompleted == false)
                _shopItemDisplayModel.ShakeTaskCompletionSource.SetResult(true);
        }

        public ShopItemDisplayModel GetShopItemModel() =>
                _shopItemDisplayModel;

        public void SetShopItem(ShopItem shopItem)
        {
            _shopItemDisplayModel.SetShopItem(shopItem);
            _shopItemDisplayModel.ShakeTaskCompletionSource.SetResult(true);
            _itemImage.sprite = shopItem.ItemSprite;
            _priceText.text = shopItem.ItemPrice.ToString();
            _purchaseButton.onClick.AddListener(() => OnPurchasePressed?.Invoke(this));
        }

        public void UpdateShopItemUI(ShopItemStateConfig shopItemStateConfig)
        {
            _purchaseParent.SetActive(shopItemStateConfig.ShopItemState == ShopItemState.NotPurchased);

            _statusParent.SetActive(shopItemStateConfig.ShopItemState != ShopItemState.NotPurchased);
            SetStatusText(shopItemStateConfig.StatusText);
        }

        public async UniTaskVoid Shake(CancellationToken token, PurchaseFailedShakeParameters purchaseFailedShakeParameters)
        {
            if (_shopItemDisplayModel.ShakeTaskCompletionSource.Task.IsCompleted == false)
                return;

            _shopItemDisplayModel.SetShakeCompletionSource(new());

            try
            {
                PlayShakeAnimation(purchaseFailedShakeParameters);
                await WaitSnakeDuration(token, purchaseFailedShakeParameters);
                token.ThrowIfCancellationRequested();
            }
            finally
            {
                _shopItemDisplayModel.ShakeTaskCompletionSource.SetResult(true);
            }
        }

        private void PlayShakeAnimation(PurchaseFailedShakeParameters purchaseFailedShakeParameters) =>
                _purchaseButton.transform
                        .DOShakePosition(
                                purchaseFailedShakeParameters.ShakeDuration,
                                purchaseFailedShakeParameters.Strength,
                                purchaseFailedShakeParameters.Vibrato,
                                purchaseFailedShakeParameters.Randomness,
                                purchaseFailedShakeParameters.Snapping,
                                purchaseFailedShakeParameters.FadeOut,
                                purchaseFailedShakeParameters.ShakeRandomnessMode)
                        .SetLink(gameObject);

        private static async UniTask WaitSnakeDuration(CancellationToken token, PurchaseFailedShakeParameters purchaseFailedShakeParameters) =>
                await UniTask.WaitForSeconds(purchaseFailedShakeParameters.ShakeDuration, cancellationToken: token);

        private void SetStatusText(string status) =>
                _statusText.text = status;

        private void SetPurchaseButtonSprite(Sprite sprite) =>
                _purchaseButton.GetComponent<Image>().sprite = sprite;
    }
}