using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExcelData;
using Player.Controller;
using UnityEngine;

public class BubbleManager : IDisposable
{
    private Dictionary<string, BubbleUI> _bubbleDict = new();
    private CancellationTokenSource _cts = new();
    private List<BubbleUI> _activeBubbles = new List<BubbleUI>(32);

    private bool _hasSkipEvent = false; 

    public bool HasSkipEvent => _hasSkipEvent;

    public BubbleManager(BubbleUI[] bubbleArray)
    {
        foreach (BubbleUI bubble in bubbleArray)
        {
            string name = bubble.transform.root.name;
            _bubbleDict.Add(name, bubble);
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }

    public async UniTask ExcuteBubble(BubbleClass bubbleData)
    {
        if(_activeBubbles.Count == 0)
        {
            _hasSkipEvent = false;
        }

        await _bubbleDict[bubbleData.Name].UpdateUI(bubbleData);

        _activeBubbles.Add(_bubbleDict[bubbleData.Name]);

        _hasSkipEvent = true;
    }

    public void CloseBubble()
    {
        for (int i = 0; i < _activeBubbles.Count; i++)
        {
            _activeBubbles[i].CloseUI();
        }
        _activeBubbles.Clear();

        _hasSkipEvent = false;
    }
}
