using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend {
    public class Cache<T> {
        // Represents one page of data.
        public struct DataPage {
            private List<T> Items { get; set; }

            /// <summary>
            ///
            /// </summary>
            /// <param name="items">The items in the page</param>
            /// <param name="rowIndex">The rowid of the first item in the page. NOT THE PAGE INDEX</param>
            public DataPage(List<T> items, int rowIndex) {
                Items = items;
                LowestIndex = rowIndex;
                HighestIndex = rowIndex + Items.Count - 1;
            }

            // Returns whether the given rowIndex is in this page.
            public bool ContainsRow(int rowIndex) {
                return rowIndex <= HighestIndex && rowIndex >= LowestIndex;
            }

            // Returns the value at the row/column indexes in this page
            public T GetValue(int rowIndex) {
                if(!ContainsRow(rowIndex)) {
                    throw new ArgumentException("The row " + rowIndex + " is not in this page.");
                }
                return Items[rowIndex - LowestIndex];
            }

            public int LowestIndex { get; private set; }
            public int HighestIndex { get; private set; }
        }

        private DataPage[] cachePages;
        private IPaginated<T> dataSupply;
        private int BlockSize;

        public Cache(IPaginated<T> dataSupplier, int blockSize) {
            dataSupply = dataSupplier;
            BlockSize = blockSize;
            LoadFirstTwoPages();
        }

        public T RetrieveElement(int rowIndex) {
            int blockIndex = GetPageContainingRow(rowIndex);
            if(blockIndex < 0) {
                CacheRow(rowIndex);
                blockIndex = GetPageContainingRow(rowIndex);
            }
            return cachePages[blockIndex].GetValue(rowIndex);
        }

        // loads and caches the row with given index
        private void CacheRow(int rowIndex) {
            int blockIndex = GetOptimalPageIndex(rowIndex);
            int pageIndex = Convert.ToInt32(Math.Floor((double)rowIndex / BlockSize));
            cachePages[blockIndex] = new DataPage(dataSupply.SupplyPageOfData(pageIndex, BlockSize), pageIndex * BlockSize);
        }

        // Finds the page index where a page containing the row with given rowIndex should be put.
        // Follows the following rules:
        // If there is a page containing this rowIndex, the index of this page is returned
        // Otherwise, the page furthest away from the rowIndex is returned.
        private int GetOptimalPageIndex(int rowIndex) {
            int blockIndex = GetPageContainingRow(rowIndex);
            if(blockIndex < 0) {
                int victim = 0;
                int victimDistance = GetDistanceToPage(rowIndex, cachePages[0]);
                for(int i = 1; i < cachePages.Count(); ++i) {
                    int distance = GetDistanceToPage(rowIndex, cachePages[i]);
                    if(distance > victimDistance) {
                        victim = i;
                        victimDistance = distance;
                    }
                }
                return victim;
            } else {
                return blockIndex;
            }
        }

        private int GetDistanceToPage(int rowIndex, DataPage page) {
            if(rowIndex > page.HighestIndex) {
                return page.HighestIndex - rowIndex;
            } else if(rowIndex < page.LowestIndex) {
                return page.LowestIndex - rowIndex;
            } else {
                throw new ArgumentException("The given page CONTAINS the row with index " + rowIndex + ", so distance makes no sense.");
            }
        }

        private int GetPageContainingRow(int rowIndex) {
            for(int i = 0; i < cachePages.Count(); ++i) {
                if(cachePages[i].ContainsRow(rowIndex)) {
                    return i;
                }
            }
            return -1;
        }

        private void LoadFirstTwoPages() {
            cachePages = new DataPage[]{
            new DataPage(dataSupply.SupplyPageOfData(0, BlockSize), 0),
            new DataPage(dataSupply.SupplyPageOfData(1, BlockSize), BlockSize)};
        }

        public void ReloadRow(int rowIndex) {
            CacheRow(rowIndex);
        }

        public void ReloadAll() {
            foreach(DataPage page in cachePages) {
                ReloadRow(page.LowestIndex);
            }
        }
    }
}