using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

public class ObjectiveUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text mainObjectiveText;
    [SerializeField] private Transform subObjectiveContainer;
    [SerializeField] private GameObject subObjectivePrefab;

    [Header("Timing")]
    [SerializeField] private float doneDisplaySeconds = 1.0f;

    private readonly Queue<string> subQueue = new Queue<string>();
    private readonly Dictionary<string, TMP_Text> checklist = new Dictionary<string, TMP_Text>();
    private TMP_Text currentSubText;
    private string currentSubRaw;
    public string CurrentSubRaw => currentSubRaw;
    public float DoneDisplaySeconds => doneDisplaySeconds;
    private bool isCompleting;

    public void SetMain(string main)
    {
        if (mainObjectiveText != null)
        {
            mainObjectiveText.text = main;
        }
    }

    public void ClearAllSubs()
    {
        subQueue.Clear();
        checklist.Clear();

        if (subObjectiveContainer != null)
        {
            for (int i = subObjectiveContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(subObjectiveContainer.GetChild(i).gameObject);
            }
            currentSubText = null;
            isCompleting = false;
        }
    }

    public void SetSubObjectivesInOrder(IEnumerable<string> subs)
    {
        ClearAllSubs();
        foreach (var s in subs)
            subQueue.Enqueue(s);
        currentSubRaw = null;

        ShowNextSub();
    }

    public void CompleteCurrentSub()
    {
        if (currentSubText == null || isCompleting) return;
        StartCoroutine(CompleteThenNext());
    }

    public bool HasMoreSubs()
    {
        return currentSubText != null || subQueue.Count > 0;
    }

    private void ShowNextSub()
    {
        if (subQueue.Count == 0)
        {
            currentSubText = null;
            currentSubRaw = null;
            return;
        }

        string next = subQueue.Dequeue();
        currentSubRaw = next;
        GameObject obj = Instantiate(subObjectivePrefab, subObjectiveContainer);
        currentSubText = obj.GetComponent<TMP_Text>();
        currentSubText.fontStyle = FontStyles.Normal;
        currentSubText.text = "[  ] " + next;
    }

    private IEnumerator CompleteThenNext()
    {
        isCompleting = true;

        string raw = currentSubText.text.StartsWith("[  ] ") ? currentSubText.text.Substring(2) : currentSubText.text;
        currentSubText.text = "[ X" + raw;
        currentSubText.fontStyle = FontStyles.Strikethrough;
        currentSubText.color = Color.gray;

        yield return new WaitForSeconds(doneDisplaySeconds);

        Destroy(currentSubText.gameObject);
        currentSubText = null;
        isCompleting = false;
        ShowNextSub();
    }

    public void SetChecklist(string main, IEnumerable<(string id, string text)> items)
    {
        SetMain(main);
        ClearAllSubs();

        checklist.Clear();

        foreach (var item in items)
        {
            GameObject obj = Instantiate(subObjectivePrefab, subObjectiveContainer);
            TMP_Text text = obj.GetComponent<TMP_Text>();
            text.fontStyle = FontStyles.Normal;
            text.color = Color.white;
            text.text = "[  ] " + item.text;

            checklist[item.id] = text;
        }
    }

    public void CheckOff(string id, float removeAfterSeconds = 0f)
    {
        if (!checklist.TryGetValue(id, out TMP_Text text) || text == null) return;

        if (text.text.StartsWith("[ X ]"))
            return;

        string raw = text.text.StartsWith("[  ] ") ? text.text.Substring(2) : text.text;
        text.text = "[ X ]" + raw;
        text.fontStyle = FontStyles.Strikethrough;
        text.color = Color.gray;

        if (removeAfterSeconds > 0f)
        {
            StartCoroutine(RemoveChecklistItemAfter(id, removeAfterSeconds));
        }
    }

    private IEnumerator RemoveChecklistItemAfter(string id, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (checklist.TryGetValue(id, out TMP_Text text) && text != null)
        {
            Destroy(text.gameObject);
        }
        checklist.Remove(id);
    }

    public bool ChecklistHas(string id)
    {
        return checklist.ContainsKey(id);
    }

    public bool IsChecklistComplete()
    {
        return checklist.Count == 0;
    }

    public void SetChecklistItemText(string id, string newText)
    {
        if (!checklist.TryGetValue(id, out TMP_Text text) || text == null) return;

        bool checkedOff = text.text.StartsWith("[ X ]");
        text.text = (checkedOff ? "[ X ]" : "[  ]  ") + newText;
    }

    public bool IsChecklistFullyChecked()
    {
        foreach (var kv in checklist)
        {
            TMP_Text text = kv.Value;
            if (text == null) continue;
            if (!text.text.StartsWith("[ X ]")) return false;
        }

        return true;
    }
}


