using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExcelData;

public class SelectManager : IDisposable
{
    private Dictionary<string, SelectUI> _selectDict = new();
    private CancellationTokenSource _cts = new();
    private List<SelectUI> _activeSelectUI = new List<SelectUI>(32);

    public SelectManager(SelectUI[] selectArray)
    {
        foreach (SelectUI selectUI in selectArray)
        {
            string name = selectUI.transform.parent.name;
            _selectDict.Add(name, selectUI);
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }


    public async UniTask<int> ExcuteSelect(SelectClass selectData, SelectDataStruct[] selectDataes)
    {
        _selectDict[selectData.Name].UpdateUI(selectData, selectDataes);

        _activeSelectUI.Add(_selectDict[selectData.Name]);

        return await _selectDict[selectData.Name].WaitSelect();
    }
}
