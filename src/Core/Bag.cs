using System.Collections;
namespace Pdcl.Core;
/// <summary>
/// A collection
/// </summary>
public sealed class Bag<T> : IEnumerable<T> where T : class
{
    private LinkedList<IDictionary<int, T>> buckets;
    public Bag()
    {
        buckets = new LinkedList<IDictionary<int, T>>();

        AddLast();
    }
    public void AddLast() => buckets.AddLast(
            new LinkedListNode<IDictionary<int, T>>(
                new Dictionary<int, T>()));
    public T? Get(int hashcode) 
    {
        foreach(IDictionary<int, T> buckets in buckets) 
        {
            if (buckets.TryGetValue(hashcode, out T? t)) 
            {
                return t;
            }
        }
        return null;
    }
    public void Insert(T t) 
    {
        buckets.Last!.Value[t.GetHashCode()] = t;
    }
    public IEnumerator<T> GetEnumerator()
    {
        foreach(IDictionary<int, T> node in buckets) 
        {
            foreach(KeyValuePair<int, T> pair in node) yield return pair.Value;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}