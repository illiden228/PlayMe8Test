using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class Scroll : PoolObject, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private RectTransform _content;
    [SerializeField] private WWWLoader _wwwLoader;
    [SerializeField] private AddressablesLoader _addressablesLoader;
    [SerializeField] private ServerEmulator _server;
    [SerializeField] private SizingCalibrator _calibrator;
    [SerializeField] private Button _nextPageButton;
    [SerializeField] private Button _previousPageButton;
    [SerializeField] private int _bufferSize;
    [SerializeField] private int _rowsCount;
    [SerializeField] private int _buttonSteps;

    private ScrollRect _scroll;
    private RectTransform _filler;
    private GridLayoutGroup _grid;
    private List<Lot> _lots = new List<Lot>();
    private int _minIndex;
    private int _maxIndex;
    private float _previousNormalizedPosition = 0;
    private bool _isSnapping = false;
    private Vector2 _fillerSnapPosition;

    private int _leftItemOutOfView { get { Debug.Log(Mathf.CeilToInt(-_content.anchoredPosition.x / _grid.cellSize.x)); return Mathf.CeilToInt(-_content.anchoredPosition.x / _grid.cellSize.x); } }
    private int _targetItemVisibleItemCount { get { return Mathf.Max(Mathf.CeilToInt(_scroll.viewport.rect.width / _grid.cellSize.x) * _rowsCount, 0); } }
    private float _xCellSizeWithSpacing { get { return _grid.cellSize.x + _grid.spacing.x; } }

    private void Start()
    {
        Init(_prefab);
        _scroll = GetComponent<ScrollRect>();
        _filler = _scroll.content;
        _grid = _content.GetComponent<GridLayoutGroup>();
        _scroll.onValueChanged.AddListener(OnScrollValueChanged);
        UpdateFromServer();
    }

    private void OnEnable()
    {
        _calibrator.SizeChanged += CalculateFillerSize;
        _nextPageButton.onClick.AddListener(OnNextPageButtonDown);
        _previousPageButton.onClick.AddListener(OnPreviousPageButtonDown);
    }

    private void OnDisable()
    {
        _calibrator.SizeChanged -= CalculateFillerSize;
        _nextPageButton.onClick.RemoveListener(OnNextPageButtonDown);
        _previousPageButton.onClick.RemoveListener(OnPreviousPageButtonDown);
    }

    public void UpdateFromServer()
    {
        CleanAll();
        _server.ListUpdated += OnListUpdated;
    }

    private void OnListUpdated(string jsonLots)
    {
        _lots = new List<Lot>();
        _lots.AddRange(JsonUtility.FromJson<JsonData<Lot>>(jsonLots).Data);

        CleanAll();
        ResetScroll();
        CalculateFillerSize();
        _server.ListUpdated -= OnListUpdated;
        SetStartItems();
    }

    private void OnScrollValueChanged(Vector2 currentNormalizedPosition)
    {
        float delta = _filler.anchoredPosition.x - _previousNormalizedPosition;
        _content.anchoredPosition = new Vector2(_content.anchoredPosition.x + delta, _content.anchoredPosition.y);
        UpdateContent();
        _previousNormalizedPosition = _filler.anchoredPosition.x;
        Debug.Log("This changed");
    }

    private void SetStartItems()
    {
        int finallyTemplateCount = _targetItemVisibleItemCount + _bufferSize * _rowsCount;
        finallyTemplateCount += _lots.Count % _rowsCount;
        for (int i = 0; i < finallyTemplateCount; i++)
        {
            if (TryGetObject(out GameObject newItem))
            {
                newItem.transform.SetParent(_content);
                newItem.SetActive(true);
                newItem.GetComponent<LotUI>().Init(_lots[_maxIndex], _wwwLoader, _addressablesLoader, _maxIndex);
                _maxIndex++;
            }
            else break;
        }
    }

    private void UpdateContent()
    {
        if(_leftItemOutOfView > _bufferSize)
        {
            if (_maxIndex >= _lots.Count) return;

            int countFreeItems = _lots.Count - _maxIndex;
            countFreeItems = countFreeItems > _rowsCount ? _rowsCount : countFreeItems;

            TransferItemsFromBegin(countFreeItems);

            _content.anchoredPosition = new Vector2(_content.anchoredPosition.x + _xCellSizeWithSpacing, _content.anchoredPosition.y);
        }
        else if (_leftItemOutOfView < _bufferSize)
        {
            if (_minIndex <= 0) return;

            TransferItemsFromEnd(_rowsCount);

            _content.anchoredPosition = new Vector2(_content.anchoredPosition.x - _xCellSizeWithSpacing, _content.anchoredPosition.y);
        }
    }

    private void TransferItemsFromEnd(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Transform lastChild = _content.GetChild(_content.childCount - 1);
            lastChild.SetSiblingIndex(0);
            _maxIndex--;
            _minIndex--;
            lastChild.GetComponent<LotUI>().Init(_lots[_minIndex], _wwwLoader, _addressablesLoader, _minIndex);
        }
    }

    private void TransferItemsFromBegin(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Transform firstChild = _content.GetChild(0);
            firstChild.SetSiblingIndex(_content.childCount - 1);
            firstChild.GetComponent<LotUI>().Init(_lots[_maxIndex], _wwwLoader, _addressablesLoader, _maxIndex);
            _maxIndex++;
            _minIndex++;
        }
    }

    private void CleanAll()
    {
        var lots = GetComponentsInChildren<LotUI>();
        foreach (var lot in lots)
        {
            if (!ReturnObject(lot.gameObject))
                Destroy(lot.gameObject);
        }
    }

    private void CalculateFillerSize()
    {
        float additionalColumn = _lots.Count % _rowsCount > 0 ? _xCellSizeWithSpacing : 0;
        _filler.sizeDelta = new Vector2(_lots.Count / _rowsCount * _xCellSizeWithSpacing - _grid.spacing.x + additionalColumn, _filler.sizeDelta.y);
    }

    private void ResetScroll()
    {
        _maxIndex = 0;
        _minIndex = 0;
        _filler.anchoredPosition = new Vector2(0, _filler.anchoredPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isSnapping = true;
        float delta = Mathf.Abs(_filler.anchoredPosition.x % _xCellSizeWithSpacing);
        if(delta < _xCellSizeWithSpacing / 2)
        {
            _fillerSnapPosition = _filler.anchoredPosition;
            _fillerSnapPosition.x += delta;
        } 
        else
        {
            _fillerSnapPosition = _filler.anchoredPosition;
            _fillerSnapPosition.x += delta - _xCellSizeWithSpacing;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isSnapping = false;
    }

    private void OnNextPageButtonDown()
    {
        Debug.Log("NextBTN");
        StartCoroutine(MoveFillerBySteps(_buttonSteps, -_xCellSizeWithSpacing));
    }

    private void OnPreviousPageButtonDown()
    {
        Debug.Log("PrevBTN");
        StartCoroutine(MoveFillerBySteps(_buttonSteps, _xCellSizeWithSpacing));
    }

    private IEnumerator MoveFillerBySteps(int count, float delta)
    {
        for (int i = 0; i < count; i++)
        {
            _filler.anchoredPosition = new Vector2(_filler.anchoredPosition.x + delta, _filler.anchoredPosition.y);
            yield return null;
        }
    }

    private void Update()
    {
        if(_isSnapping)
        {
            if (_filler.anchoredPosition.x != _fillerSnapPosition.x)
            {
                _filler.anchoredPosition = _fillerSnapPosition;
            }
            else
                _isSnapping = false;
        }
    }
}
