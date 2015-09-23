using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageTools.Backend {
    public interface ISpecification<T> {

        bool IsSatisfiedBy(T entity);
    }

    public interface ISqlSpecification<T> : ISpecification<T> {
        string Sql { get; }
        Dictionary<string, object> Parameters { get; }
    }
}