using System;
using System.Collections.Generic;

[Serializable]
public class ChatBotHistory
{
    private List<Tuple<string, List<Tuple<string, string, bool>>>> data = new List<Tuple<string, List<Tuple<string, string, bool>>>>();

    public int Add(string dateTime, string message, string detailTime, bool isUser)
    {
        string lastDateTime = data.Count > 0 ? data[data.Count - 1].Item1 : "";

        if (lastDateTime == dateTime)
        {
            data[data.Count - 1].Item2.Add(new Tuple<string, string, bool>(message, detailTime, isUser));
        }
        else
        {
            List<Tuple<string, string, bool>> messages = new List<Tuple<string, string, bool>>();
            messages.Add(new Tuple<string, string, bool>(message, detailTime, isUser));
            data.Add(new Tuple<string, List<Tuple<string, string, bool>>>(dateTime, messages));
        }

        return data[data.Count - 1].Item2.Count - 1;
    }

    public List<Tuple<string, string, bool>> GetDataByIndex(int index)
    {
        return data[index].Item2;
    }

    public string GetDateTimeByIndex(int index)
    {
        return data[index].Item1;
    }

    public int Count()
    {
        return data.Count;
    }

    public override string ToString()
    {
        string result = "";
        foreach (Tuple<string, List<Tuple<string, string, bool>>> item in data)
        {
            result += item.Item1 + "\n";
            foreach (Tuple<string, string, bool> message in item.Item2)
            {
                result += message.Item1 + "\n";
            }
        }
        return result;
    }
}
