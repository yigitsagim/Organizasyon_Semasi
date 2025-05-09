using System;
using System.Collections.Generic;
using System.Linq;  // LINQ metodlarını kullanmak için EKLENDİ

public class CalisanHashTable
{
    private int size = 100;
    private Node[] table;

    public CalisanHashTable()
    {
        table = new Node[size];
    }

    private int Hash(string id)
    {
        return Math.Abs(id.GetHashCode()) % size;
    }

    public void Ekle(Node node)
    {
        int index = Hash(node.ID);
        if (table[index] == null)
        {
            table[index] = node;
        }
        else
        {
            Node current = table[index];
            while (current.Next != null)
                current = current.Next;
            current.Next = node;
        }
    }

    public Node Get(string id)
    {
        int index = Hash(id);
        Node current = table[index];
        while (current != null)
        {
            if (current.ID == id)
                return current;
            current = current.Next;
        }
        return null;
    }

    public void Remove(string id)
    {
        int index = Hash(id);
        Node current = table[index];
        Node prev = null;

        while (current != null)
        {
            if (current.ID == id)
            {
                if (prev == null)
                    table[index] = current.Next;
                else
                    prev.Next = current.Next;
                return;
            }
            prev = current;
            current = current.Next;
        }
    }

    public List<Node> TumCalisanlariGetir()
    {
        var list = new List<Node>();
        foreach (var head in table)
        {
            Node current = head;
            while (current != null)
            {
                list.Add(current);
                current = current.Next;
            }
        }
        return list;
    }

    public bool YoneticiVarMi(string departman)
    {
        return TumCalisanlariGetir().Any(n =>
            n.Departman == departman &&
            n.Unvan == "Yönetici" &&
            !string.IsNullOrWhiteSpace(n.Ad) &&
            !string.IsNullOrWhiteSpace(n.Soyad));
    }

    public Node GetYonetici(string departman)
    {
        return TumCalisanlariGetir().FirstOrDefault(n =>
            n.Departman == departman && n.Unvan == "Yönetici");
    }

    public void BosYoneticileriSil(string departman)
    {
        var bosYoneticiler = TumCalisanlariGetir()
            .Where(n => n.Departman == departman && n.Unvan == "Yönetici" &&
                        string.IsNullOrWhiteSpace(n.Ad) && string.IsNullOrWhiteSpace(n.Soyad))
            .Select(n => n.ID)
            .ToList();

        foreach (var id in bosYoneticiler)
            Remove(id);
    }

    public void Clear()
    {
        for (int i = 0; i < size; i++)
        {
            table[i] = null;
        }
    }
}

