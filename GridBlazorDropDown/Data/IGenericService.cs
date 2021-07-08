using GridShared;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridBlazorDropDown.Data
{
    public interface IGenericService<T> : ICrudDataService<T>
    {
        ItemsDTO<T> Get(Action<IGridColumnCollection<T>> columns, QueryDictionary<StringValues> query);
        IEnumerable<SelectItem> GetSelect();
        IEnumerable<SelectItem> GetSelect(string search);
    }
}
