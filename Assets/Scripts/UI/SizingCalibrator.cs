using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SizingCalibrator : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    private RectTransform _imageRect;
    private RectTransform _canvasRect;
    private RectTransform _containerRect;
    private GridLayoutGroup _grid;

    private Vector2 _currentCanvasSize = new Vector2();
    public event UnityAction SizeChanged;

    void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        _imageRect = GetComponent<RectTransform>();
        _canvasRect = canvas.GetComponent<RectTransform>();
        _containerRect = _scrollRect.GetComponent<RectTransform>();
        _grid = _scrollRect.GetComponentInChildren<GridLayoutGroup>();
        CalibrateWindowSize();
        CalibrateCellSize();
    }

    void Update()
    {
        if (_currentCanvasSize.x != _canvasRect.rect.height && _currentCanvasSize.y != _canvasRect.rect.width)
        {
            CalibrateWindowSize();
            CalibrateCellSize();
            SizeChanged?.Invoke();
        }
    }

    private void CalibrateWindowSize()
    {
        float imageRelatio = _imageRect.rect.height / _imageRect.rect.width;
        float canvasRelatio = _canvasRect.rect.height / _canvasRect.rect.width;
        _currentCanvasSize.x = _canvasRect.rect.height;
        _currentCanvasSize.y = _canvasRect.rect.width;
        if (canvasRelatio < imageRelatio)
            _imageRect.sizeDelta = new Vector2(_canvasRect.rect.height / imageRelatio, _canvasRect.rect.height);
        else
            _imageRect.sizeDelta = new Vector2(_canvasRect.rect.width, _canvasRect.rect.width * imageRelatio);
    }

    private void CalibrateCellSize()
    {
        _grid.cellSize = new Vector2(_containerRect.rect.width / 2f - 2f, _containerRect.rect.height / 3f - 1f);
    }
}
