using Dapper;

namespace ChiDaram.Common.Classes
{
    public class MdsGridViewDapperModel
    {
        public MdsGridViewDapperModel()
        {
            WhereString = "";
            SortString = "";
            Params = new DynamicParameters();
        }
        public string WhereString { get; set; }
        public string SortString { get; set; }
        public DynamicParameters Params { get; set; }
    }
}
