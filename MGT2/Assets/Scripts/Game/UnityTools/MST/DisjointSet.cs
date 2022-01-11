public class DisjointSet
{
    public int[] root;
    public int[] size;

    public DisjointSet(int n)
    {
        this.root = new int[n];
        this.size = new int[n];
        for (int i = 0; i < n; i++)
        {
            root[i] = i;
            size[i] = 1;
        }

    }

    public int Find(int x)
    {
        if (root[x] != x)
        {
            root[x] = Find(root[x]);
        }
        return root[x];
    }

    public void Union(int x, int y)
    {
        int rootX = Find(x);
        int rootY = Find(y);
        if (rootX == rootY)
        {
            return;
        }
        if (size[rootX] < size[rootY])
        {
            root[rootX] = rootY;
            size[rootY]++;
            return;
        }
        root[rootY] = rootX;
        size[rootX]++;
    }
}