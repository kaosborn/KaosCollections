//
// Library: KaosCollections
// File:    BtreeFind.cs
// Purpose: Define BtreeDictionary seek function without a NodeVector path.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System.Collections.Generic;

namespace Kaos.Collections
{
    public partial class BtreeDictionary<TKey,TValue>
    {
        #region Nonpublic methods

        /// <summary>Perform lite search for key.</summary>
        /// <param name="key">Target of search.</param>
        /// <param name="index">When found, holds index of returned Leaf; else ~index of nearest greater key.</param>
        /// <returns>Leaf holding target (found or not).</returns>
        private Leaf Find (TKey key, out int index)
        {
            //  Unfold on default comparer for 5% speed improvement.
            if (comparer == Comparer<TKey>.Default)
                for (Node node = root;;)
                {
                    index = node.Search (key);

                    if (node is Branch branch)
                        node = branch.GetChild (index < 0 ? ~index : index + 1);
                    else
                        return (Leaf) node;
                }
            else
                for (Node node = root;;)
                {
                    index = node.Search (key, comparer);

                    if (node is Branch branch)
                        node = branch.GetChild (index < 0 ? ~index : index + 1);
                    else
                        return (Leaf) node;
                }
        }

        #endregion
    }
}
