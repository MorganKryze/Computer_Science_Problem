namespace Instances;

/// <summary> Represents a Huffman tree. </summary>
public class HuffmanTree
{
    /// <summary> The root of the tree. </summary>
    public HuffmanNode root;

    /// <summary> The constructor of the Huffman tree. </summary>
    public HuffmanTree(int[] frequencies)
    {
        var nodes = new List<HuffmanNode>();

        for (int i = 0; i < frequencies.Length; i++)
            if (frequencies[i] > 0)
                nodes.Add(new HuffmanNode { Value = i, Frequency = frequencies[i] });

        while (nodes.Count > 1)
        {
            nodes.Sort((x, y) => x.Frequency.CompareTo(y.Frequency));

            var left = nodes[0];
            var right = nodes[1];

            nodes.Remove(left);
            nodes.Remove(right);

            var parent = new HuffmanNode { Value = -1, Frequency = left.Frequency + right.Frequency, Left = left, Right = right };

            nodes.Add(parent);
        }

        root = nodes.Count > 0 ? nodes[0] : new();
    }
    /// <summary> Generates the binary codes for the tree. </summary>
    public Dictionary<int, string> GenerateCodes()
    {
        var codes = new Dictionary<int, string>();

        if (root != null)
            BuildCodesDictionary(root, "", codes);

        return codes;
    }

    private void BuildCodesDictionary(HuffmanNode node, string code, Dictionary<int, string> codes)
    {
        if (node.Value != -1)
            codes[node.Value] = code;
        else
        {
            BuildCodesDictionary(node.Left ?? new(), code + "0", codes);
            BuildCodesDictionary(node.Right ?? new(), code + "1", codes);
        }
    }
}
