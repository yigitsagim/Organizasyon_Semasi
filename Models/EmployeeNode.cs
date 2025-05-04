namespace veri_yapilari.Models;

public class EmployeeNode
{
    public string Key { get; set; }
    public string Parent { get; set; }
    public string Text { get; set; }
    public List<EmployeeNode> Children { get; set; } = new List<EmployeeNode>();
}
