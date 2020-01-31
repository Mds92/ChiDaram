using System;

namespace ChiDaram.Common.Helper
{
    public static class DapperHelper
    {
        public static string AddTsqlWhereString(this string whereConditionString, string condition)
        {
            var tsqlExpression = whereConditionString;
            if (whereConditionString.IndexOf("WHERE", StringComparison.InvariantCulture) == -1)
                tsqlExpression += $" WHERE {condition}";
            else
                tsqlExpression += $" AND {condition}";
            return tsqlExpression + " ";
        }
    }
}
