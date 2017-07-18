//
// Library: KaosCollections
// File:    DebugView.cs
// Purpose: Define API for Debug telemetry and debugger helpers.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Kaos.Collections
{
    /// <exclude />
    internal class IDictionaryDebugView<K,V>
    {
        private readonly IDictionary<K,V> target;

        public IDictionaryDebugView (IDictionary<K,V> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException (nameof (dictionary));
            target = dictionary;
        }

        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<K,V>[] Items
        {
            get
            {
                KeyValuePair<K,V>[] items = new KeyValuePair<K,V>[target.Count];
                target.CopyTo (items, 0);
                return items;
            }
        }
    }

    /// <exclude />
    internal class ICollectionKeysDebugView<K,V>
    {
        private readonly ICollection<K> target;

        public ICollectionKeysDebugView (ICollection<K> collection)
        {
            if (collection == null)
                throw new ArgumentNullException (nameof (collection));
            target = collection;
        }

        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        public K[] Items
        {
            get
            {
                K[] items = new K[target.Count];
                target.CopyTo (items, 0);
                return items;
            }
        }
    }


    /// <exclude />
    internal class ICollectionValuesDebugView<K,V>
    {
        private readonly ICollection<V> target;

        public ICollectionValuesDebugView (ICollection<V> collection)
        {
            if (collection == null)
                throw new ArgumentNullException (nameof (collection));
            target = collection;
        }

        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        public V[] Items
        {
            get
            {
                V[] items = new V[target.Count];
                target.CopyTo (items, 0);
                return items;
            }
        }
    }

    /// <exclude />
    internal class ICollectionDebugView<T>
    {
        private readonly ICollection<T> target;

        public ICollectionDebugView (ICollection<T> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException (nameof (dictionary));
            target = dictionary;
        }

        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] items = new T[target.Count];
                target.CopyTo (items, 0);
                return items;
            }
        }
    }


#if DEBUG
    public partial class RankedDictionary<TKey,TValue>
    {
        // Telemetry counters:

        /// <summary>Maximum number of keys that the existing branches can hold.</summary>
        public int BranchSlotCount { get; private set; }
        /// <summary>Number of keys contained in the branches.</summary>
        public int BranchSlotsUsed { get; private set; }
        /// <summary>Maximum number of keys that the existing leaves can hold.</summary>
        public int LeafSlotCount { get; private set; }
        /// <summary>Number of keys contained in the leaves.</summary>
        public int LeafSlotsUsed { get; private set; }


        /// <summary>
        /// Perform diagnostics check for data structure sanity errors. Since this is an
        /// in-memory managed structure, any errors would indicate a bug. Also performs space
        /// complexity diagnostics to ensure that all non-rightmost nodes maintain 50% fill.
        /// </summary>
        public void SanityCheck()
        {
            BranchSlotCount = 0;
            BranchSlotsUsed = 0;
            LeafSlotCount = 0;
            LeafSlotsUsed = 0;

            Leaf lastLeaf;
            if (root is Branch)
                lastLeaf = CheckBranch ((Branch) root, 1, GetHeight(), true, default (TKey), null);
            else
                lastLeaf = CheckLeaf ((Leaf) root, true, default (TKey), null);

            if (lastLeaf.RightLeaf != null)
                throw new InvalidOperationException ("Last leaf has invalid RightLeaf");

            if (Count != LeafSlotsUsed)
                throw new InvalidOperationException ("Mismatched Count=" + Count + ", expected=" + LeafSlotsUsed);
        }


        /// <summary>Maximum number of children of a branch.</summary>
        /// <returns>Maximum number of children of a branch.</returns>
        public int GetOrder()
        { return maxKeyCount + 1; }


        /// <summary>Return the number of levels in the tree.</summary>
        /// <returns>Number of levels in the tree.</returns>
        public int GetHeight()
        {
            int level = 1;
            for (Node node = root; node is Branch; node = ((Branch) node).Child0)
                ++level;
            return level;
        }


        private Leaf CheckBranch
        (
            Branch branch,
            int level, int height,
            bool isRightmost,
            TKey anchor,  // ignored when isRightmost true
            Leaf visited
        )
        {
            BranchSlotCount += maxKeyCount;
            BranchSlotsUsed += branch.KeyCount;

            if (! isRightmost && (branch.KeyCount + 1) < maxKeyCount / 2)
                throw new InvalidOperationException ("Branch underfilled");

            if (branch.ChildCount != branch.KeyCount + 1)
                throw new InvalidOperationException ("Branch mismatched ChildCount, KeyCount");

            int actualWeight = 0;
            for (int i = 0; i < branch.ChildCount; ++i)
            {
                TKey anchor0 = i == 0 ? anchor : branch.GetKey (i - 1);
                bool isRightmost0 = isRightmost && i < branch.ChildCount;
                if (i < branch.KeyCount - 1)
                    if (branch.GetKey (i).CompareTo (branch.GetKey (i + 1)) >= 0)
                        throw new InvalidOperationException ("Branch keys not ascending");

                if (level + 1 < height)
                    visited = CheckBranch ((Branch) branch.GetChild (i), level+1, height, isRightmost0, anchor0, visited);
                else
                    visited = CheckLeaf ((Leaf) branch.GetChild (i), isRightmost0, anchor0, visited);

                actualWeight += branch.GetChild (i).Weight;
            }
            if (branch.Weight != actualWeight)
                throw new InvalidOperationException ("Branch mismatched weight");

            return visited;
        }


        private Leaf CheckLeaf (Leaf leaf, bool isRightmost, TKey anchor, Leaf visited)
        {
            LeafSlotCount += maxKeyCount;
            LeafSlotsUsed += leaf.KeyCount;

            if (leaf.RightLeaf != null && leaf.KeyCount < (maxKeyCount + 1) / 2)
                throw new InvalidOperationException ("Leaf underfilled");

            if (! anchor.Equals (default (TKey)) && ! anchor.Equals (leaf.Key0))
                throw new InvalidOperationException ("Leaf has wrong anchor");

            for (int i = 0; i < leaf.KeyCount; ++i)
                if (i < leaf.KeyCount - 1 && leaf.GetKey (i).CompareTo (leaf.GetKey (i + 1)) >= 0)
                    throw new InvalidOperationException ("Leaf keys not ascending");

            if (visited == null)
            {
                if (! anchor.Equals (default (TKey)))
                    throw new InvalidOperationException ("Inconsistent visited, anchor");
            }
            else
                if (visited.RightLeaf != leaf)
                    throw new InvalidOperationException ("Leaf has bad RightLeaf");

            return leaf;
        }


        /// <summary>Return telemetry summary.</summary>
        /// <returns>Telemetry summary.</returns>
        public string GetTreeStatsText()
        {
            SanityCheck();
            string result = "--- height = " + GetHeight();

            if (BranchSlotCount != 0)
                result += ", branch fill = " + (int) (BranchSlotsUsed * 100.0 / BranchSlotCount + 0.5) + "%";

            return result + ", leaf fill = " + (int) (LeafSlotsUsed * 100.0 / LeafSlotCount + 0.5) + "%";
        }


        /// <summary>Generate contents of tree by level (breadth first).</summary>
        /// <returns>Text lines where each line is a level of the tree.</returns>
        public IEnumerable<string> GenerateTreeText (bool showWeight=false)
        {
            int level = 0;
            Node leftmost;
            var sb = new StringBuilder();

            for (;;)
            {
                var branchPath = new NodeVector (this, level);
                leftmost = branchPath.TopNode;
                if (leftmost is Leaf)
                    break;

                var branch = (Branch) leftmost;

                sb.Append ('B');
                sb.Append (level);
                sb.Append (": ");
                for (;;)
                {
                    branch.Append (sb);
                    if (showWeight)
                    {
                        sb.Append (" (");
                        sb.Append (branch.Weight);
                        sb.Append (") ");
                    }

                    branch = (Branch) branchPath.TraverseRight();
                    if (branch == null)
                        break;

                    sb.Append (" | ");
                }
                ++level;
                yield return sb.ToString();
                sb.Length = 0;
            }

            var leafPath = new NodeVector (this, level);
            sb.Append ('L');
            sb.Append (level);
            sb.Append (": ");
            for (var leaf = (Leaf) leftmost;;)
            {
                leaf.Append (sb);
                leaf = (Leaf) leafPath.TraverseRight();
                if (leaf == null)
                    break;

                if (leafPath.Child0)
                    sb.Append (" | ");
                else
                    sb.Append ('|');
            }
            yield return sb.ToString();
        }
    }
#endif
}
