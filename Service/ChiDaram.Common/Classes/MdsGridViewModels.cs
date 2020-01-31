using System.Collections.Generic;

namespace ChiDaram.Common.Classes
{
    public class TsqlGridViewConditions
    {
        public TsqlGridViewConditions()
        {
            ItemsPerPage = int.MaxValue;
            WhereConditions = "";
            SortConditions = "";
        }
        public string SortConditions { get; set; }
        public string WhereConditions { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
    }

    public class MdsGridViewModel<T> where T : class
    {
        public List<T> Rows { get; set; }
        public int CurrentPage { get; set; }
        public int TotalRows { get; set; }
    }
}
