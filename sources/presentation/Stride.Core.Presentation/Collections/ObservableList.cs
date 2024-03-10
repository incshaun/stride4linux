// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
//using System.Windows.Documents;
using Stride.Core.Annotations;

namespace Stride.Core.Presentation.Collections
{
    public class FakeList<T> : IObservableList<T>, IReadOnlyObservableList<T>
    {
        public T this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void AddRange([NotNull] IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class ObservableList<T> : NonGenericObservableListWrapper<T>, IObservableList<T>, IReadOnlyObservableList<T>
    {
        private readonly List<T> list;

        //[CollectionAccess(CollectionAccessType.None)]
        public ObservableList ([NotNull] IObservableList<T> list = null)
            : base(new FakeList<T>())
        {
            if (list != null)
            {
                this.list = new List<T>(list);
            }
            else 
            {
                this.list = new List<T>();

            }
            List = this;
        }

        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        public ObservableList([NotNull] IEnumerable<T> collection) : base (new FakeList<T>())
        {
            this.list = new List<T>(collection);
            List = this;
        }

        [CollectionAccess(CollectionAccessType.None)]
        public ObservableList(int capacity) : base (new FakeList<T>())
        {
            this.list = new List<T>(capacity);
            List = this;
        }

        public T this[int index]
        {
            [CollectionAccess(CollectionAccessType.Read)]
            get { return list[index]; }
            [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
            set
            {
                var oldItem = list[index];
                list[index] = value;
                var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index);
                OnCollectionChanged(arg);
            }
        }

        [CollectionAccess(CollectionAccessType.None)]
        public int Count => list.Count;

        [CollectionAccess(CollectionAccessType.None)]
        public bool IsReadOnly => false;

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [NotNull]
        public IList ToIList()
        {
            return new NonGenericObservableListWrapper<T>(this);
        }

        [Pure]
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        [Pure]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        public void Add(T item)
        {
            Insert(Count, item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            var itemList = items.ToList();
            if (itemList.Count > 0)
            {
                list.AddRange(itemList);

                var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemList, Count - itemList.Count);
                OnCollectionChanged(arg);
            }
        }

        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        public void Clear()
        {
            var raiseEvent = list.Count > 0;
            list.Clear();
            if (raiseEvent)
            {
                var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                OnCollectionChanged(arg);
            }
        }

        [CollectionAccess(CollectionAccessType.Read)]
        [Pure]
        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        [CollectionAccess(CollectionAccessType.Read)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        [CollectionAccess(CollectionAccessType.Read)]
        public int FindIndex([NotNull] Predicate<T> match)
        {
            return list.FindIndex(match);
        }

        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        public bool Remove(T item)
        {
            int index = list.IndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
            }
            return index != -1;
        }

        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        public void RemoveRange(int index, int count)
        {
            var oldItems = list.Skip(index).Take(count).ToList();
            list.RemoveRange(index, count);
            var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, index);
            OnCollectionChanged(arg);
        }

        [CollectionAccess(CollectionAccessType.Read)]
        [Pure]
        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        public void Insert(int index, T item)
        {
            list.Insert(index, item);
            var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
            OnCollectionChanged(arg);
        }

        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        public void RemoveAt(int index)
        {
            var item = list[index];
            list.RemoveAt(index);
            var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
            OnCollectionChanged(arg);
        }

        /// <inheritdoc/>
        [CollectionAccess(CollectionAccessType.None)]
        public override string ToString()
        {
            return $"{{ObservableList}} Count = {Count}";
        }

        protected void OnCollectionChanged([NotNull] NotifyCollectionChangedEventArgs arg)
        {
            CollectionChanged?.Invoke(this, arg);

            switch (arg.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                    break;
            }
        }

        protected void OnPropertyChanged([NotNull] PropertyChangedEventArgs arg)
        {
            PropertyChanged?.Invoke(this, arg);
        }
    }
}
