using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Streetcred.Sdk.Models.Records.Search
{
    public class PagedCollection<T> : Collection<T>
    {
        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="PagedCollection{T}" /> class.</summary>
        public PagedCollection()
        {
        }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="PagedCollection{T}" /> class.</summary>
        /// <param name="list">The list that is wrapped by the new collection.</param>
        public PagedCollection(IList<T> list) : base(list)
        {
        }

        /// <summary>Gets or sets the index of the page.</summary>
        /// <value>The index of the page.</value>
        public int PageIndex { get; set; }


        /// <summary>
        /// Gets the total result count
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        public int TotalCount { get; internal set; }


        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <value>
        /// The item count.
        /// </value>
        public int ItemCount => Items.Count;
    }
}