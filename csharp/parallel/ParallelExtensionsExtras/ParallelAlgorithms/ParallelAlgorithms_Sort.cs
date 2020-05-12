//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Threading.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        /// <summary>Sorts an array in parallel.</summary>
        /// <typeparam name="T">Specifies the type of data in the array.</typeparam>
        /// <param name="array">The array to be sorted.</param>
        public static void Sort<T>(T[] array) =>
            Sort(array, (IComparer<T>)null);

        /// <summary>Sorts an array in parallel.</summary>
        /// <typeparam name="T">Specifies the type of data in the array.</typeparam>
        /// <param name="array">The array to be sorted.</param>
        /// <param name="comparer">The comparer used to compare two elements during the sort operation.</param>
        public static void Sort<T>(T[] array, IComparer<T> comparer)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            Sort<T, object>(array, null, 0, array.Length, comparer);
        }

        /// <summary>Sorts an array in parallel.</summary>
        /// <typeparam name="T">Specifies the type of data in the array.</typeparam>
        /// <param name="array">The array to be sorted.</param>
        /// <param name="index">The index at which to start the sort, inclusive.</param>
        /// <param name="length">The number of elements to be sorted, starting at the start index.</param>
        public static void Sort<T>(T[] array, int index, int length) =>
            Sort<T, object>(array, null, index, length, (IComparer<T>)null);

        /// <summary>Sorts an array in parallel.</summary>
        /// <typeparam name="T">Specifies the type of data in the array.</typeparam>
        /// <param name="array">The array to be sorted.</param>
        /// <param name="index">The index at which to start the sort, inclusive.</param>
        /// <param name="length">The number of elements to be sorted, starting at the start index.</param>
        /// <param name="comparer">The comparer used to compare two elements during the sort operation.</param>
        public static void Sort<T>(T[] array, int index, int length, IComparer<T> comparer) =>
            Sort<T, object>(array, null, index, length, comparer);

        /// <summary>Sorts key/value arrays in parallel.</summary>
        /// <typeparam name="TKey">Specifies the type of the data in the keys array.</typeparam>
        /// <typeparam name="TValue">Specifies the type of the data in the items array.</typeparam>
        /// <param name="keys">The keys to be sorted.</param>
        /// <param name="items">The items to be sorted based on the corresponding keys.</param>
        public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items) =>
            Sort(keys, items, 0, keys.Length, (IComparer<TKey>)null);

        /// <summary>Sorts key/value arrays in parallel.</summary>
        /// <typeparam name="TKey">Specifies the type of the data in the keys array.</typeparam>
        /// <typeparam name="TValue">Specifies the type of the data in the items array.</typeparam>
        /// <param name="keys">The keys to be sorted.</param>
        /// <param name="items">The items to be sorted based on the corresponding keys.</param>
        /// <param name="comparer">The comparer used to compare two elements during the sort operation.</param>
        public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items, IComparer<TKey> comparer)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            Sort(keys, items, 0, keys.Length, comparer);
        }

        /// <summary>Sorts key/value arrays in parallel.</summary>
        /// <typeparam name="TKey">Specifies the type of the data in the keys array.</typeparam>
        /// <typeparam name="TValue">Specifies the type of the data in the items array.</typeparam>
        /// <param name="keys">The keys to be sorted.</param>
        /// <param name="items">The items to be sorted based on the corresponding keys.</param>
        /// <param name="index">The index at which to start the sort, inclusive.</param>
        /// <param name="length">The number of elements to be sorted, starting at the start index.</param>
        public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items, int index, int length) =>
            Sort(keys, items, index, length, (IComparer<TKey>)null);

        /// <summary>Sorts key/value arrays in parallel.</summary>
        /// <typeparam name="TKey">Specifies the type of the data in the keys array.</typeparam>
        /// <typeparam name="TValue">Specifies the type of the data in the items array.</typeparam>
        /// <param name="keys">The keys to be sorted.</param>
        /// <param name="items">The items to be sorted based on the corresponding keys.</param>
        /// <param name="index">The index at which to start the sort, inclusive.</param>
        /// <param name="length">The number of elements to be sorted, starting at the start index.</param>
        /// <param name="comparer">The comparer used to compare two elements during the sort operation.</param>
        public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items, int index, int length, IComparer<TKey> comparer)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            if (index < 0 || length < 0) throw new ArgumentOutOfRangeException(length < 0 ? nameof(length) : nameof(index));
            if (keys.Length - index < length || items != null && index > items.Length - length) throw new ArgumentException("index");

            // Run the core sort operation
            new Sorter<TKey, TValue>(keys, items, comparer).QuickSort(index, index + length - 1);
        }

        // Stores the data necessary for the sort, and provides the core sorting method
        private sealed class Sorter<TKey, TItem>
        {
            private readonly TKey[] _keys;
            private readonly TItem[] _items;
            private readonly IComparer<TKey> _comparer;

            public Sorter(TKey[] keys, TItem[] items, IComparer<TKey> comparer)
            {
                _keys = keys;
                _items = items;
                _comparer = comparer ?? Comparer<TKey>.Default;
            }

            // Gets a recommended depth for recursion.  This assumes that every level will
            // spawn two child tasks, which isn't actually the case with the algorithm, but
            // it's a "good enough" approximation.
            private static int GetMaxDepth() => (int)Math.Log(Environment.ProcessorCount, 2);

            // Swaps the items at the two specified indexes if they need to be swapped
            internal void SwapIfGreaterWithItems(int a, int b)
            {
                if (a != b)
                {
                    if (_comparer.Compare(_keys[a], _keys[b]) > 0)
                    {
                        TKey temp = _keys[a];
                        _keys[a] = _keys[b];
                        _keys[b] = temp;
                        if (_items != null)
                        {
                            TItem item = _items[a];
                            _items[a] = _items[b];
                            _items[b] = item;
                        }
                    }
                }
            }

            // Gets the middle value between the provided low and high
            private static int GetMiddle(int low, int high) => low + ((high - low) >> 1);

            // Does a quicksort of the stored data, between the positions (inclusive specified by left and right)
            internal void QuickSort(int left, int right) => QuickSort(left, right, 0, GetMaxDepth());

            // Does a quicksort of the stored data, between the positions (inclusive specified by left and right).
            // Depth specifies the current recursion depth, while maxDepth specifies the maximum depth
            // we should recur to until we switch over to sequential.
            internal void QuickSort(int left, int right, int depth, int maxDepth)
            {
                const int SEQUENTIAL_THRESHOLD = 0x1000;

                // If the max depth has been reached or if we've hit the sequential
                // threshold for the input array size, run sequential.
                if (depth >= maxDepth || right - left + 1 <= SEQUENTIAL_THRESHOLD)
                {
                    Array.Sort(_keys, _items, left, right - left + 1, _comparer);
                    return;
                }

                // Store all tasks generated to process subarrays
                List<Task> tasks = new List<Task>();

                // Run the same basic algorithm used by Array.Sort, but spawning Tasks for all recursive calls
                do
                {
                    int i = left;
                    int j = right;

                    // Pre-sort the low, middle (pivot), and high values in place.
                    int middle = GetMiddle(i, j);
                    SwapIfGreaterWithItems(i, middle); // swap the low with the mid point
                    SwapIfGreaterWithItems(i, j);      // swap the low with the high
                    SwapIfGreaterWithItems(middle, j); // swap the middle with the high

                    // Get the pivot
                    TKey x = _keys[middle];

                    // Move all data around the pivot value
                    do
                    {
                        while (_comparer.Compare(_keys[i], x) < 0) i++;
                        while (_comparer.Compare(x, _keys[j]) < 0) j--;

                        if (i > j) break;
                        if (i < j)
                        {
                            TKey key = _keys[i];
                            _keys[i] = _keys[j];
                            _keys[j] = key;
                            if (_items != null)
                            {
                                TItem item = _items[i];
                                _items[i] = _items[j];
                                _items[j] = item;
                            }
                        }
                        i++;
                        j--;
                    } while (i <= j);

                    if (j - left <= right - i)
                    {
                        if (left < j)
                        {
                            int leftcopy = left, jcopy = j;
                            tasks.Add(Task.Factory.StartNew(() => QuickSort(leftcopy, jcopy, depth + 1, maxDepth)));
                        }
                        left = i;
                    }
                    else
                    {
                        if (i < right)
                        {
                            int icopy = i, rightcopy = right;
                            tasks.Add(Task.Factory.StartNew(() => QuickSort(icopy, rightcopy, depth + 1, maxDepth)));
                        }
                        right = j;
                    }
                } while (left < right);

                // Wait for all of this level's tasks to complete
                Task.WaitAll(tasks.ToArray());
            }
        }
    }
}
