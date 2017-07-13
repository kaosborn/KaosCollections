//
// Library: KaosCollections
// File:    BtreeExtras.cs
// Purpose: Define methods that do not have corresponding definitions in SortedDictionary.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;

namespace Kaos.Collections
{
    public partial class BtreeDictionary<TKey,TValue>
    {
        /// <summary>
        /// Get the Last key/value pair without performing a full structure scan.
        /// </summary>
        /// <returns>Key/value pair with largest key in dictionary</returns>
        public KeyValuePair<TKey,TValue> Last()
        {
            if (Count == 0)
                throw new InvalidOperationException ("Sequence contains no elements.");

            // Take rightmost child until no more.
            for (Node node = root;;)
            {
                var branch = node as Branch;
                if (branch == null)
                    return new KeyValuePair<TKey,TValue> (node.GetKey (node.KeyCount - 1),
                                                          ((Leaf) node).GetValue (node.KeyCount - 1));

                node = branch.GetChild (node.KeyCount);
            }
        }


        /// <summary>
        /// This iterator provides range query support with ordered results.
        /// </summary>
        /// <param name="key">Minimum value of range.</param>
        /// <returns>An enumerator for the collection for key values greater than or equal to <em>key</em>.</returns>
        /// <exception cref="ArgumentNullException">When supplied key is <b>null</b>.</exception>
        public IEnumerable<KeyValuePair<TKey,TValue>> SkipUntilKey (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            Leaf leaf = Find (key, out int index);

            // When the supplied start key is not be found, start with the next highest key.
            if (index < 0)
                index = ~index;

            for (;;)
            {
                if (index < leaf.KeyCount)
                {
                    yield return leaf.GetPair (index);
                    ++index;
                    continue;
                }

                leaf = leaf.RightLeaf;
                if (leaf == null)
                    yield break;

                index = 0;
            }
        }


        /// <summary>
        /// This iterator provides range query support.
        /// </summary>
        /// <param name="startKey">Minimum inclusive key value of range.</param>
        /// <param name="endKey">Maximum inclusive key value of range.</param>
        /// <returns>An enumerator for all key/value pairs between startKey and endKey.</returns>
        /// <remarks>
        /// Neither <em>startKey</em> or <em>endKey</em> need to be present in the collection.
        /// </remarks>
        /// <example>
        /// <code source="..\Bench\BtreeExample03\BtreeExample03.cs" lang="cs" />
        /// </example>
        public IEnumerable<KeyValuePair<TKey,TValue>> BetweenKeys (TKey startKey, TKey endKey)
        {
            Leaf leaf = Find (startKey, out int index);

            // When the supplied start key is not be found, start with the next highest key.
            if (index < 0)
                index = ~index;

            for (;;)
            {
                if (index < leaf.KeyCount)
                {
                    if (leaf.GetKey (index).CompareTo (endKey) > 0)
                        yield break;

                    yield return leaf.GetPair (index);
                    ++index;
                    continue;
                }

                leaf = leaf.RightLeaf;
                if (leaf == null)
                    yield break;

                index = 0;
            }
        }


        public int IndexOf (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            var path = new NodeVector (this, key);
            if (! path.IsFound)
                return -1;

            return path.GetIndex();
        }


        public KeyValuePair<TKey,TValue> GetByIndex (int index)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException (nameof (index), "Specified argument was out of the range of valid values.");

            Node node = root;
            while (node is Branch)
                for (int ix = 0; ix <= node.KeyCount; ++ix)
                {
                    var child = ((Branch) node).GetChild (ix);
                    int cw = child.Weight;
                    if (cw > index)
                    {
                        node = child;
                        break;
                    }
                    index -= cw;
                }

            return new KeyValuePair<TKey,TValue> (((Leaf) node).GetKey (index), ((Leaf) node).GetValue (index));
        }


        public bool TryGetValueAndIndex (TKey key, out TValue value, out int index)
        {
            var path = new NodeVector (this, key);
            if (! path.IsFound)
            {
                value = default (TValue);
                index = -1;
                return false;
            }

            value = path.LeafValue;
            index = path.GetIndex();
            return true;
        }
    }
}
