using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using veri_yapilari.Models;

namespace veri_yapilari.Controllers
{
    public class OrgChartController : Controller
    {
        public IActionResult GetChartData()
{
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "company.csv");
    var nodes = new List<Dictionary<string, string>>();

    using (var reader = new StreamReader(filePath))
    {
        bool isFirstLine = true;
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (isFirstLine) { isFirstLine = false; continue; }

            var parts = line.Split(',');
            if (parts.Length >= 3)
            {
                nodes.Add(new Dictionary<string, string>
                {
                    ["key"] = parts[0],
                    ["parent"] = string.IsNullOrWhiteSpace(parts[1]) ? null : parts[1],
                    ["text"] = parts[2]
                });
            }
        }
    }

    var json = JsonSerializer.Serialize(nodes);
    return Content(json, "application/json");
}

    }
}
