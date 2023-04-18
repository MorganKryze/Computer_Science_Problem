namespace Instances;

/// <summary> Represents a node in a Huffman tree. </summary>
public class HuffmanNode
{
    /// <summary> The value of the node. </summary>
    public int Value { get; set; }
    /// <summary> The frequency of the node. </summary>
    public int Frequency { get; set; }
    /// <summary> The left child of the node. </summary>
    public HuffmanNode? Left { get; set; }
    /// <summary> The right child of the node. </summary>
    public HuffmanNode? Right { get; set; }
}

