using System.Reflection;
using System.Text;

namespace AbfSharp.Reports;

public class AbfHeaderReport
{
    readonly Item[] Items;

    struct Item
    {
        public string Name;
        public string Type;
        public string Value;
    }

    public AbfHeaderReport(string abfFilePath)
    {
        ABF abf = new(abfFilePath, preloadSweepData: false);

        List<Item> items = [];

        FieldInfo[] fields = abf.Header.GetType().GetFields();
        foreach (FieldInfo fi in fields)
        {
            object structElementValue = abf.Header.GetType().GetField(fi.Name).GetValue(abf.Header);
            if (structElementValue.GetType().IsArray)
            {
                List<string> values = new();
                int length = ((Array)structElementValue).Length;
                foreach (object arrayValue in (Array)structElementValue)
                    values.Add(arrayValue.ToString());
                if (values.Count > 20)
                {
                    values = values.Take(20).ToList();
                    values.Add("...");
                }
                string typeName = structElementValue.GetType().ToString().Replace("[]", $"[{length}]").Replace("System.", "");

                Item item = new()
                {
                    Name = fi.Name,
                    Type = typeName,
                    Value = string.Join(", ", values)
                };

                items.Add(item);
            }
            else if (structElementValue.GetType() == typeof(string))
            {
                string s = structElementValue.ToString();

                Item item = new()
                {
                    Name = fi.Name,
                    Type = $"String[{s.Length}]",
                    Value = s
                };

                items.Add(item);
            }
            else
            {
                Item item = new()
                {
                    Name = fi.Name,
                    Type = structElementValue.GetType().ToString().Replace("System.", ""),
                    Value = structElementValue.ToString()
                };

                items.Add(item);
            }
        }

        Items = items.ToArray();
    }

    public string GetMarkdown()
    {
        StringBuilder sb = new();
        sb.AppendLine("Name | Type | Value");
        sb.AppendLine("---|---|---");
        foreach (var item in Items)
        {
            string value = item.Value
                .Replace("\r", @"\r")
                .Replace("\n", @"\n");

            sb.AppendLine($"{item.Name} | {item.Type} | {value}");
        }

        return sb.ToString();
    }

    public string GetHtml()
    {
        StringBuilder sb = new();
        sb.AppendLine("<html><body>");
        sb.AppendLine("<table>");
        sb.AppendLine("<tr style='font-weight: bold;'>");
        sb.AppendLine("<td>Name</td>");
        sb.AppendLine("<td>Type</td>");
        sb.AppendLine("<td>Value</td>");
        sb.AppendLine("</tr>");
        foreach (var item in Items)
        {
            sb.AppendLine("<tr>");
            sb.AppendLine($"<td>{item.Name}</td>");
            sb.AppendLine($"<td>{item.Type}</td>");
            sb.AppendLine($"<td>{item.Value}</td>");
            sb.AppendLine("</tr>");
        }
        sb.AppendLine("</table>");
        sb.AppendLine("</body></html>");

        return sb.ToString();
    }
}
