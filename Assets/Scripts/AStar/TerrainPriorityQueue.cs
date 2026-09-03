using UnityEngine;
using System;
using System.Collections.Generic;
//https://www.redblobgames.com/pathfinding/a-star/introduction.html - A* Algorithm
//https://www.geeksforgeeks.org/dsa/a-search-algorithm/
//https://medium.com/@nanda.yugandhar/a-vs-dijkstra-a-visual-guide-to-why-a-sense-of-direction-matters-ef9378d71a53 - Research
//https://www.redblobgames.com/pathfinding/a-star/implementation.html - More on  A* Algorithm

// https://www.geeksforgeeks.org/dsa/priority-queue-using-binary-heap/Binary-heap - Priority queue using heap .
//https://codesignal.com/learn/courses/advanced-built-in-data-structures-and-their-usage-2/lessons/queues-and-deques-in-csharp
//https://www.c-sharpcorner.com/blogs/wrapper-class-in-c-sharp1

// TElement and TPriorty are generic types so element is TerrainCell (Vector2Int) and TPrority is the double priortity value for A*
public class TerrainPriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>  
{
    private readonly List<(TElement element, TPriority priority)> heap = new List<(TElement, TPriority)>();

    public int Count => heap.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        heap.Add((element, priority));
        int i = heap.Count - 1;

        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (heap[parent].priority.CompareTo(heap[i].priority) <= 0)
                break;

            (heap[parent], heap[i]) = (heap[i], heap[parent]);
            i = parent;
        }
    }

    public TElement Dequeue()
    {
        (TElement element, TPriority priority) root = heap[0];
        int lastIndex = heap.Count - 1;

        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);
        lastIndex--;

        int i = 0;
        while (true)
        {
            int left = i * 2 + 1;
            int right = i * 2 + 2;
            int smallest = i;

            if (left <= lastIndex && heap[left].priority.CompareTo(heap[smallest].priority) < 0)
                smallest = left;
            if (right <= lastIndex && heap[right].priority.CompareTo(heap[smallest].priority) < 0)
                smallest = right;

            if (smallest == i)
                break;

            (heap[i], heap[smallest]) = (heap[smallest], heap[i]);
            i = smallest;
        }

        return root.element;
    }
}
