using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AbfSharp.Reports;

public static class AbfHeaderReport
{
    public static string GetMarkdown(ABFFIO.AbfFileHeader HeaderStruct)
    {
        StringBuilder sb = new();
        sb.AppendLine("Name | Type | Value");
        sb.AppendLine("---|---|---");

        FieldInfo[] fields = HeaderStruct.GetType().GetFields();
        foreach (FieldInfo fi in fields)
        {
            object structElementValue = HeaderStruct.GetType().GetField(fi.Name).GetValue(HeaderStruct);
            if (structElementValue.GetType().IsArray)
            {
                List<string> vals = new();
                int length = ((Array)structElementValue).Length;
                foreach (object arrayValue in (Array)structElementValue)
                    vals.Add(arrayValue.ToString());
                if (vals.Count > 20)
                {
                    vals = vals.Take(20).ToList();
                    vals.Add("...");
                }
                string typeName = structElementValue.GetType().ToString().Replace("[]", $"[{length}]").Replace("System.", "");
                sb.AppendLine($"{fi.Name} | {typeName} | {string.Join(", ", vals)}");
            }
            else if (structElementValue.GetType() == typeof(string))
            {
                string s = (string)structElementValue;
                sb.AppendLine($"{fi.Name} | string ({s.Length}) | \"{s}\"");
            }
            else
            {
                string typeName = structElementValue.GetType().ToString().Replace("System.", "");
                sb.AppendLine($"{fi.Name} | {typeName} | {structElementValue}");
            }
        }

        return sb.ToString();
    }
}
