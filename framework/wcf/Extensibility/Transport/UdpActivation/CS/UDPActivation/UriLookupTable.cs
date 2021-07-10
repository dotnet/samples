//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ServiceModel.Samples
{
    class UriTreeNode<T>
        where T : class
    {
        T item;
        string key;
        Dictionary<string, UriTreeNode<T>> children;
        public UriTreeNode(string key)
        {
            this.key = key;
            this.children = new Dictionary<string, UriTreeNode<T>>(StringComparer.OrdinalIgnoreCase);
        }

        public T Item
        {
            get
            {
                return this.item;
            }

            set
            {
                this.item = value;
            }
        }

        public UriTreeNode<T> FindChild(string key)
        {
            UriTreeNode<T> child;
            if (children.TryGetValue(key, out child))
            {
                return child;
            }

            return null;
        }

        public void AddChild(string key, UriTreeNode<T> child)
        {
            children[key] = child;
        }

        public bool Remove(string[] segments)
        {
            return Remove(segments, 0);
        }

        // Remove all children.
        bool Remove(string[] segments, int depth)
        {
            if (segments.Length == depth)
            {
                this.item = null;
                return this.children.Count == 0;
            }

            UriTreeNode<T> child;
            if (children.TryGetValue(segments[depth], out child))
            {
                if (child.Remove(segments, depth + 1))
                {
                    children.Remove(segments[depth]);
                    return this.children.Count == 0 && this.item == null;
                }
            }

            return false;
        }
    }

    // Format: /port/apppath
    class UriLookupTable<T>
        where T : class
    {
        UriTreeNode<T> root;
        public UriLookupTable()
        {
            this.root = new UriTreeNode<T>(null);
        }

        public void Add(Uri uri, T item)
        {
            UriTreeNode<T> node = FindOrCreate(uri, true);
            node.Item = item;
        }

        UriTreeNode<T> FindOrCreate(Uri uri, bool createNew)
        {
            List<string> segments = GetSegments(uri);

            UriTreeNode<T> current = root;
            lock (ThisLock)
            {
                for (int i = 0; i < segments.Count; i++)
                {
                    UriTreeNode<T> next = current.FindChild(segments[i]);
                    if (next == null)
                    {
                        if (createNew)
                        {
                            next = new UriTreeNode<T>(segments[i]);
                            current.AddChild(segments[i], next);
                        }
                        else
                        {
                            return current;
                        }
                    }

                    current = next;
                }
            }

            return current;
        }

        public T Lookup(Uri uri)
        {
            UriTreeNode<T> node = FindOrCreate(uri, false);
            return node.Item;
        }

        public bool Remove(Uri uri)
        {
            List<string> segments = GetSegments(uri);
            return root.Remove(segments.ToArray());
        }

        static List<string> GetSegments(Uri uri)
        {
            List<string> segments = new List<string>();

            // We ignore uri.Host for simplicity.
            segments.Add(uri.Port.ToString(NumberFormatInfo.InvariantInfo));
            for (int i = 0; i < uri.Segments.Length; i++)
            {
                if (uri.Segments[i] == "/")
                    continue;

                segments.Add(uri.Segments[i].Trim('/'));
            }

            return segments;
        }

        object ThisLock
        {
            get
            {
                return this.root;
            }
        }
    }
}

