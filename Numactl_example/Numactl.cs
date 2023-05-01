public class InorderIterator {
    private Node current, rightMost;
    public InorderIterator(Node root);
    public bool HasNext();
    public Node Next();
    public Node CurrentItem();
}

public class PostorderIterator {
    private Node current, rightMost;
    public PostorderIterator(Node root);
    public bool HasNext();
    public Node Next();
    public Node CurrentItem();
}