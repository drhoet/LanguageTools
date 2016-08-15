using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend {
    public interface IPaginated<T> {
        List<T> SupplyPageOfData(int pageIndex, int pageSize);
    }
}