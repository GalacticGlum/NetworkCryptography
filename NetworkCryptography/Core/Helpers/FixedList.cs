/*
 * Author: Shon Verch
 * File Name: FixedList.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/20/2017
 * Modified Date: 10/2017
 * Description: Represents a list with a fixed capacity. Throws out elements from the bottom when it reaches the max size (to make room for new elements).
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace NetworkCryptography.Core.Helpers
{
    /// <summary>
    /// Represents a list with a fixed capacity. Throws out elements from the bottom when it reaches the max size (to make room for new elements).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FixedList<T> : IList<T>
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        public T this[int index]
        {
            get => items[index];
            set => items[index] = value;
        }

        /// <summary>
        /// The maximum amount of elements that the <see cref="FixedList{T}"/> can hold.
        /// </summary>
        public int MaxSize { get; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="FixedList{T}"/>.
        /// </summary>
        public int Count => items.Count;

        /// <summary>
        /// Is this list read-only?
        /// </summary>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// The internal backing list data-structure.
        /// </summary>
        private readonly List<T> items;

        /// <summary>
        /// The pointer index where the next element will go in.
        /// </summary>
        private int nextIndexPointer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedList{T}"/> class that is empty and has the specified fixed size.
        /// </summary>
        /// <param name="maxSize"></param>
        public FixedList(int maxSize)
        {
            MaxSize = maxSize;
            items = new List<T>(maxSize);
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="FixedList{T}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="FixedList{T}"/>. The value can be null for reference types.</param>
        public void Add(T item)
        {
            // If we have exceeded our max size, set our pointer to 0 so it resets to the start.
            if (Count >= MaxSize)
            {
                nextIndexPointer = 0;
            }

            if (Count >= nextIndexPointer + 1)
            {
                items[nextIndexPointer++] = item;
            }
            else
            {
                items.Add(item);
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="FixedList{T}"/>.
        /// </summary>
        public void Clear() => items.Clear();

        /// <summary>
        /// Determines whether an element is in the <see cref="FixedList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="FixedList{T}"/>. The value can be null for reference types.</param>
        public bool Contains(T item) => items.Contains(item);

        /// <summary>
        /// Copies the entire <see cref="FixedList{T}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements from <see cref="FixedList{T}"/>. 
        /// The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> which copying begins.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentException"/>
        public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes the first occurence of a specific object from the <see cref="FixedList{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="FixedList{T}"/>. The value can be null for reference types.</param>
        public bool Remove(T item) => items.Remove(item);

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurence within the entire <see cref="FixedList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="FixedList{T}"/>. The value can be null for reference types.</param>
        public int IndexOf(T item) => items.IndexOf(item);

        /// <summary>
        /// Inserts an element into the <see cref="FixedList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void Insert(int index, T item) => items.Insert(index, item);

        /// <summary>
        /// Removes the element at the specified index of the <see cref="FixedList{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void RemoveAt(int index) => items.RemoveAt(index);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="FixedList{T}"/>.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="FixedList{T}"/>.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
