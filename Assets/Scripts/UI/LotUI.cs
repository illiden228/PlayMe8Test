using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LotUI: MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private RawImage _playerIcon;
    [SerializeField] private Image _moneyIcon;

    [SerializeField] private TMP_Text _nameItem;
    [SerializeField] private TMP_Text _countItem;
    [SerializeField] private TMP_Text _namePlayer;
    [SerializeField] private TMP_Text _levelPlayer;
    [SerializeField] private TMP_Text _countMoney;

    [SerializeField] private Text _test;

    public event UnityAction Deactivated;

    private Transform _container;

    public void Init(Lot lot, WWWLoader loader, AddressablesLoader addressablesLoader, int test)
    {
        loader.LoadImage(lot.Player.IconURL, _playerIcon);
        addressablesLoader.LoadSprite(lot.Item.Icon, _itemIcon);
        addressablesLoader.LoadSprite(lot.Item.MoneyType.Icon, _moneyIcon);

        _nameItem.text = lot.Item.Name;
        _countItem.text = "x" + lot.ItemCount;
        _namePlayer.text = lot.Player.Name;
        _levelPlayer.text = lot.Player.Level.ToString();
        _countMoney.text = (lot.ItemCount * lot.Item.Cost).ToString();
        _test.text = test.ToString();
        gameObject.name = test.ToString();
    }
}
