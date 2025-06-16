using Core;
using UnityEngine;

namespace Runtime.Application.ShopSystem
{
    [CreateAssetMenu(fileName = "ShopItemsConfig", menuName = "Config/ShopItemsConfig")]
    public class ShopItemStateConfig : BaseSettings
    {
        [SerializeField, Space(20)] private string _statusText;
        [SerializeField] private ShopItemState _shopItemState;
        [SerializeField] private Sprite _stateSprite;
        public string StatusText => _statusText;
        public ShopItemState ShopItemState => _shopItemState;
        public Sprite StateSprite => _stateSprite;
    }
}