using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ServerEmulator : MonoBehaviour
{
    [SerializeField] private List<Player> _players;
    [SerializeField] private List<Item> _items;
    [SerializeField] private string _jsonData;
    [SerializeField] private int _count;

    public event UnityAction<string> ListUpdated;

    private void Start()
    {
        StartCoroutine(StartServer());
    }

    public IEnumerator StartServer()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            List<Lot> result = new List<Lot>();
            
            for (int i = 0; i < _count; i++)
            {
                Lot newLot = new Lot(_items[Random.Range(0, _items.Count)], _players[Random.Range(0, _players.Count)], Random.Range(1, 100));
                result.Add(newLot);
            }

            JsonData<Lot> data = new JsonData<Lot>(result);
            _jsonData = JsonUtility.ToJson(data);

            ListUpdated?.Invoke(_jsonData);
            yield return new WaitForSeconds(10f);
        }
    }

    public void GetData()
    {
        //return _jsonData;
    }

    private IEnumerator DownloadData()
    {
        yield return new WaitForSeconds(1f);


    }
}

public class JsonData<T>
{
    public List<T> Data;

    public JsonData(List<T> data)
    {
        Data = data;
    }
}
