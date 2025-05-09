public class Node
{
    public string ID { get; set; }
    public string Departman { get; set; }
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public string Unvan { get; set; }
    public string ParentID { get; set; }
    public Node Next { get; set; }

    public Node(string id, string departman, string ad, string soyad, string unvan, string parentId)
    {
        ID = id;
        Departman = departman;
        Ad = ad;
        Soyad = soyad;
        Unvan = unvan;
        ParentID = parentId;
        Next = null;
    }
}
