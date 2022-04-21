
public struct ItemPos3
{
    public int l; // layer: 层
    public int c; // col: 行
    public int r; // raw: 列


    public ItemPos3(int[] data)
    {
        if( data == null)
        {
            l = 0;
            c = 0;
            r = 0;
        }
        else
        {
            l = data.Length > 0 ? data[0] : 0;
            c = data.Length > 1 ? data[1] : 0;
            r = data.Length > 2 ? data[2] : 0;
        }
    }
    public ItemPos3(int l, int c, int r)
    {
        this.l = l;
        this.c = c;
        this.r = r;
    }

    public int[] ToArray()
    {
        return new int[] { l, c, r };
    }

    public static bool operator ==(ItemPos3 a, ItemPos3 b)
    {
        return a.l == b.l && a.c == b.c && a.r == b.r;
    }
    public static bool operator !=(ItemPos3 a, ItemPos3 b)
    {
        return a.l != b.l || a.c != b.c || a.r != b.r;
    }

    public static ItemPos3 operator +(ItemPos3 a, ItemPos3 b)
    {
        ItemPos3 c;
        c.l = a.l + b.l;
        c.c = a.c + b.c;
        c.r = a.r + b.r;
        return c;
    }

    public static ItemPos3 operator -(ItemPos3 a, ItemPos3 b)
    {
        ItemPos3 c;
        c.l = a.l - b.l;
        c.c = a.c - b.c;
        c.r = a.r - b.r;
        return c;
    }
}


public struct SizeInt2
{
    public int width;
    public int height;


    public SizeInt2(int[] data)
    {
        if (data == null)
        {
            width = 0;
            height = 0;
        }
        else
        {
            width = data.Length > 0 ? data[0] : 0;
            height = data.Length > 1 ? data[1] : 0;
        }
    }
    public SizeInt2(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public int[] ToArray()
    {
        return new int[] { width, height };
    }
}
    public struct ItemPos2
{
    public int c; // col: 行
    public int r; // raw: 列


    public ItemPos2(int[] data)
    {
        if (data == null)
        {
            c = 0;
            r = 0;
        }
        else
        {
            c = data.Length > 0 ? data[0] : 0;
            r = data.Length > 1 ? data[1] : 0;
        }
    }
    public ItemPos2(int c, int r)
    {
        this.c = c;
        this.r = r;
    }

    public int[] ToArray()
    {
        return new int[] {c, r };
    }

}