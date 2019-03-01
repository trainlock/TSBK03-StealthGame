using UnityEngine;
using System.Collections;
using System;

public interface IHeapItem<T> : IComparable<T>{
    int HeapIndex{
        get;
        set;
    }
}

public class Heap<T> where T : IHeapItem<T>{

    T[] items;
    int currentItemCount;

    // Constructor for the heap
    public Heap(int maxHeapSize){
        items = new T[maxHeapSize];
    }

    // Add item to heap
    public void Add(T item){
        // Add item last in item heap
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;

        // Sort item according to priority
        SortUp(item);

        // Increment current item count with 1
        currentItemCount++;
    }

    // Remove the first item in heap
    public T RemoveFirst(){
        T firstItem = items[0];
        currentItemCount--;

        // Put the item at the end of the heap and put it in the first place
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;

        // Sort the heap
        SortDown(items[0]);
        return firstItem;
    }

    // Change priority of the item
    public void UpdateItem(T item){
        SortUp(item);
    }

    // Get number of items currently in the heap
    public int Count{
        get{
            return currentItemCount;
        }
    }

    // Check if heap contains a specific item
    public bool Contains(T item){
        return Equals(items[item.HeapIndex], item);
    }

    void SortDown(T item){
        while (true){
            // Get indices of the two children
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            // Check if item have one child
            if (childIndexLeft < currentItemCount){
                swapIndex = childIndexLeft;

                // Check if item have a second child
                if (childIndexRight < currentItemCount){
                    // Check which of the two children that have the highest priority
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0){
                        swapIndex = childIndexRight;
                    }
                }

                // Check if parent have a lower priority than its child
                if (item.CompareTo(items[swapIndex]) < 0){
                    Swap(item, items[swapIndex]);
                }
                else{
                    // If parent have a higher priority than both of its children then it is in the right position
                    return;
                }
            }
            else{
                // If the parent doesn't have any children then it is in the right position
                return;
            }

        }
    }

    // Move item up in heap
    void SortUp(T item){
        // Get index of partent to item
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true){
            T parentItem = items[parentIndex];

            // Check if item have a lower priority than parent then swap them
            if (item.CompareTo(parentItem) > 0){
                Swap(item, parentItem);
            }
            else{
                break;
            }

            // Compare item to its new parent
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    // Swap places between item A and item B
    void Swap(T itemA, T itemB){
        // Swap values in array
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        // Swap heap index
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}