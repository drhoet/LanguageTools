using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager {
    class Cache {

        // Represents one page of data.   
        public struct DataPage {
            private DataTable Table { get; set; }

            public DataPage(DataTable table, int rowIndex) {
                Table = table;
                LowestIndex = rowIndex;
                HighestIndex = rowIndex + Table.Rows.Count - 1;
            }

            // Returns whether the given rowIndex is in this page.
            public bool ContainsRow(int rowIndex) {
                return rowIndex <= HighestIndex &&
                    rowIndex >= LowestIndex;
            }

            // Returns the value at the row/column indexes in this page
            public object GetValue(int rowIndex, int columnIndex) {
                if(!ContainsRow(rowIndex)) {
                    throw new ArgumentException("The row " + rowIndex + " is not in this page.");
                }
                return Table.Rows[rowIndex - LowestIndex][columnIndex];
            }
            
            public int LowestIndex { get; private set; }
            public int HighestIndex { get; private set; }
        }

        private DataPage[] cachePages;
        private DataProvider dataSupply;
        private int RowsPerPage;

        public Cache(DataProvider dataSupplier, int rowsPerPage) {
            dataSupply = dataSupplier;
            RowsPerPage = rowsPerPage;
            LoadFirstTwoPages();
        }
    
        public object RetrieveElement(int rowIndex, int columnIndex) {
            int pageIndex = GetPageContainingRow(rowIndex);
            if(pageIndex < 0) {
                CacheRow(rowIndex);
                pageIndex = GetPageContainingRow(rowIndex);
            }
            return cachePages[pageIndex].GetValue(rowIndex, columnIndex);
        }

        // loads and caches the row with given index
        private void CacheRow(int rowIndex) {
            int pageIndex = GetOptimalPageIndex(rowIndex);
            int pageLowerBoundary = (rowIndex / RowsPerPage) * RowsPerPage;
            cachePages[pageIndex] = new DataPage(dataSupply.SupplyPageOfData(pageLowerBoundary, RowsPerPage), pageLowerBoundary);
        }

        // Finds the page index where a page containing the row with given rowIndex should be put.
        // Follows the following rules:
        // If there is a page containing this rowIndex, the index of this page is returned
        // Otherwise, the page furthest away from the rowIndex is returned.
        private int GetOptimalPageIndex(int rowIndex) {
            int pageIndex = GetPageContainingRow(rowIndex);
            if(pageIndex < 0) {
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
                return pageIndex;
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
            new DataPage(dataSupply.SupplyPageOfData(0, RowsPerPage), 0),
            new DataPage(dataSupply.SupplyPageOfData(RowsPerPage, RowsPerPage), RowsPerPage)};
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
