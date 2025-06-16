using UnityEngine;

namespace Runtime.Application.ShopSystem
{
    [CreateAssetMenu(fileName = "Shop Item", menuName = "Config/Shop Item")]
    public class ShopItem : ScriptableObject
    {
        [SerializeField] private int _itemID;
        [SerializeField] private int _itemPrice;
        [SerializeField] private Sprite _itemSprite;
        [SerializeField] private ShopItemDisplayView _shopItemDisplayView;

        public int ItemID => _itemID;
        public int ItemPrice => _itemPrice;
        public Sprite ItemSprite => _itemSprite;
        public ShopItemDisplayView ShopItemDisplayView => _shopItemDisplayView;
    }
}