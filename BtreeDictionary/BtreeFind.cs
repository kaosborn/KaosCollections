//
// Library: KaosCollections
// File:    BtreeFind.cs
// Purpose: Define BtreeDictionary seek functions without a TreePath.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System.Collections.Generic;

namespace Kaos.Collections
{
    public partial class BtreeDictionary<TKey, TValue>
    {
        #region Nonpublic methods

        /// <summary>Get the leftmost leaf.</summary>
        /// <remarks>Used by iteration.</remarks>
        private Leaf GetFirstLeaf()
        {
            for (Node node = root; ; node = ((Branch) node).FirstChild)
                if (node is Leaf)
                    return (Leaf) node;
        }


        /// <summary>Perform lite search for key.</summary>
        /// <param name="key">Target of search.</param>
        /// <param name="index">When found, holds index of returned Leaf; else ~index of nearest greater key.</param>
        /// <returns>Leaf holding target (found or not).</returns>
        private Leaf Find (TKey key, out int index)
        {
            //  Method is unfolded on comparer to improve speed 5%.
            if (comparer == Comparer<TKey>.Default)
                for (Node node = root;;)
                {
                    int nodeIndex = node.Search (key);

                    var branch = node as Branch;
                    if (branch == null)
                    {
                        index = nodeIndex;
                        return (Leaf) node;
                    }

                    node = branch.GetChild (nodeIndex < 0 ? ~nodeIndex : nodeIndex + 1);
                }
            else
            {
                for (Node node = root;;)
                {
                    int nodeIndex = node.Search (key, comparer);

                    var branch = node as Branch;
                    if (branch == null)
                    {
                        index = nodeIndex;
                        return (Leaf) node;
                    }

                    node = branch.GetChild (nodeIndex < 0 ? ~nodeIndex : nodeIndex + 1);
                }
            }
        }

        #endregion
    }
}
