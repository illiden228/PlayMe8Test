using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockUI : MonoBehaviour
{
    [SerializeField] private Button _stockButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _updateButton;

    [SerializeField] private Image _blur;
    [SerializeField] private SizingCalibrator _calibrator;
    [SerializeField] private WWWLoader _wwwLoader;
    [SerializeField] private AddressablesLoader _addressablesLoader;
    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private Scroll _scroll;

    private void Start()
    {
        OnClickCloseButton();
    }

    private void OnEnable()
    {
        _stockButton.onClick.AddListener(OnClickStockButton);
        _closeButton.onClick.AddListener(OnClickCloseButton);
        _updateButton.onClick.AddListener(_scroll.UpdateFromServer);
    }

    private void OnDisable()
    {
        _stockButton.onClick.RemoveListener(OnClickStockButton);
        _closeButton.onClick.RemoveListener(OnClickCloseButton);
        _updateButton.onClick.RemoveListener(_scroll.UpdateFromServer);
    }

    private void OnClickStockButton()
    {
        _blur.enabled = true;
        _calibrator.enabled = true;
        _scroll.enabled = true;
        _wwwLoader.enabled = true;
        _addressablesLoader.enabled = true;
        _panel.alpha = 1f;
        _panel.interactable = true;
        _panel.blocksRaycasts = true;
    }

    private void OnClickCloseButton()
    {
        _blur.enabled = false;
        _calibrator.enabled = false;
        _scroll.enabled = false;
        _wwwLoader.enabled = false;
        _addressablesLoader.enabled = false;
        _panel.alpha = 0f;
        _panel.interactable = false;
        _panel.blocksRaycasts = false;
    }
}

