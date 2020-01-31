using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ChiDaram.Common.Classes;
using ChiDaram.Common.Enums;
using Dapper;

namespace ChiDaram.Common.Helper
{
    public static class MdsGridViewHelper
    {
        // α == and
        // β == or

        private static readonly List<Type> DateTimeTypes = new List<Type>
        {
            typeof(DateTime),
            typeof(DateTime?)
        };
        //private static readonly List<Type> EnumerableTypes = new List<Type>
        //{
        //    typeof(IEnumerable<>),
        //    typeof(IEnumerator<>),
        //    typeof(IList<>),
        //    typeof(List<>),
        //};

        public static int GetFirstRowNumber(int page, int itemsPerPage)
        {
            return (page - 1) * itemsPerPage + 1;
        }
        public static int GetLastRowNumber(int page, int itemsPerPage)
        {
            var lastRowNumber = itemsPerPage * page;
            return lastRowNumber <= 0 ? itemsPerPage : lastRowNumber;
        }
        public static MdsGridViewModel<Dictionary<object, object>> GetMdsGridViewModel<T>(TsqlGridViewConditions model,
            Func<TsqlGridViewConditions, int> selectCount,
            Func<TsqlGridViewConditions, List<T>> selectAll,
            Action<T> processRecord = null)
        {
            var gridViewModel = new MdsGridViewModel<Dictionary<object, object>>
            {
                CurrentPage = model.Page,
                Rows = new List<Dictionary<object, object>>(),
                TotalRows = selectCount(model)
            };
            var rowNumber = GetFirstRowNumber(model.Page, model.ItemsPerPage);
            if (gridViewModel.TotalRows <= model.ItemsPerPage) model.Page = 1;
            var records = selectAll(model);
            if (records.Count <= 0)
            {
                model.Page = 1;
                gridViewModel.CurrentPage = 1;
                records = selectAll(model);
            }
            foreach (var record in records)
            {
                processRecord?.Invoke(record);
                var dictionary = GetDictionaries(record);
                dictionary.Add("#", rowNumber.ToString().ToCurrentCultureNumber());
                gridViewModel.Rows.Add(dictionary);
                rowNumber++;
            }
            return gridViewModel;
        }

        private static string ReadyForLike(string inputString)
        {
            return inputString.Replace("[", "[[]").Replace("%", "[%]");
        }
        private static List<string> ReadyForContain(string inputString)
        {
            return inputString.Replace("[", "").Replace("]", "").SplitOnChars(',').Distinct().ToList();
        }
        public static string GetAndTsqlCondition(string propertyName, OperatorEnum operatorEnum, string valueString)
        {
            return $"α{propertyName}={valueString}¤{(int)operatorEnum}β";
        }
        private static TsqlCondition GetTsqlCondition(string propertyName, OperatorEnum operatorEnum, string valueString, string parameterName)
        {
            var tsql = "";
            object parameterValue = valueString;
            switch (operatorEnum)
            {
                case OperatorEnum.Equal:
                    {
                        if (valueString.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                            tsql = $"{propertyName} IS NULL";
                        else
                            tsql = $"{propertyName} = {parameterName}";
                    }
                    break;

                case OperatorEnum.NotEqual:
                    {
                        if (valueString.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                            tsql = $"{propertyName} NOT NULL OR ";
                        else
                            tsql = $"{propertyName} != {parameterName}";
                    }
                    break;

                case OperatorEnum.Contain:
                    {
                        tsql = $"{propertyName} IN {parameterName}";
                        parameterValue = ReadyForContain(valueString);
                    }
                    break;

                case OperatorEnum.NotContain:
                    {
                        tsql = $"{propertyName} NOT IN {parameterName}";
                        parameterValue = ReadyForContain(valueString);
                    }
                    break;

                case OperatorEnum.Like:
                    {
                        tsql = $"{propertyName} LIKE {parameterName}";
                        parameterValue = $"%{ReadyForLike(valueString)}%";
                    }
                    break;

                case OperatorEnum.NotLike:
                    {
                        tsql = $"{propertyName} NOT LIKE {parameterName}";
                        parameterValue = $"%{ReadyForLike(valueString)}%";
                    }
                    break;

                case OperatorEnum.StartsWith:
                    {
                        tsql = $"{propertyName} LIKE {parameterName}";
                        parameterValue = $"%{ReadyForLike(valueString)}";
                    }
                    break;

                case OperatorEnum.NotStartsWith:
                    {
                        tsql = $"{propertyName} NOT LIKE {parameterName}";
                        parameterValue = $"%{ReadyForLike(valueString)}";
                    }
                    break;

                case OperatorEnum.EndsWith:
                    {
                        tsql = $"{propertyName} LIKE {parameterName}";
                        parameterValue = $"{ReadyForLike(valueString)}%";
                    }
                    break;

                case OperatorEnum.NotEndsWith:
                    {
                        tsql = $"{propertyName} NOT LIKE {parameterName}";
                        parameterValue = $"{ReadyForLike(valueString)}%";
                    }
                    break;

                case OperatorEnum.GreaterThan:
                    {
                        tsql = $"{propertyName} > {parameterName}";
                        parameterValue = valueString;
                    }
                    break;

                case OperatorEnum.GreaterThanOrEqual:
                    {
                        tsql = $"{propertyName} >= {parameterName}";
                        parameterValue = valueString;
                    }
                    break;

                case OperatorEnum.LessThan:
                    {
                        tsql = $"{propertyName} < {parameterName}";
                        parameterValue = valueString;
                    }
                    break;

                case OperatorEnum.LessThanOrEqual:
                    {
                        tsql = $"{propertyName} <= {parameterName}";
                        parameterValue = valueString;
                    }
                    break;

                case OperatorEnum.IsNull:
                    {
                        tsql = $"{propertyName} IS NULL";
                    }
                    break;

                case OperatorEnum.IsNotNull:
                    {
                        tsql = $"{propertyName} IS NOT NULL OR ";
                    }
                    break;
            }
            return new TsqlCondition
            {
                ParameterValue = parameterValue,
                Tsql = tsql
            };
        }
        private static MdsGridViewDapperModel GetTsqlCondition(OperatorEnum operatorEnum, string valueString, string parameterName, params string[] propertyNames)
        {
            string conditionString;
            var tsqlCondition = new TsqlCondition();
            var parameters = new DynamicParameters();
            if (propertyNames.Length > 1)
            {
                conditionString = "(";
                foreach (var propertyName in propertyNames)
                {
                    tsqlCondition = GetTsqlCondition(propertyName, operatorEnum, valueString, parameterName);
                    conditionString += tsqlCondition.Tsql + " OR ";
                }
                conditionString = conditionString.Remove(conditionString.LastIndexOf("OR", StringComparison.InvariantCulture));
                conditionString += ")";
            }
            else
            {
                tsqlCondition = GetTsqlCondition(propertyNames[0], operatorEnum, valueString, parameterName);
                conditionString = tsqlCondition.Tsql;
            }
            parameters.Add(parameterName, tsqlCondition.ParameterValue);
            return new MdsGridViewDapperModel
            {
                WhereString = conditionString,
                Params = parameters
            };
        }
        private static MdsGridViewDapperModel GetMdsGridViewWhereConditions(string conditionsString, IReadOnlyDictionary<string, string> aliasDictionary)
        {
            if (string.IsNullOrWhiteSpace(conditionsString)) return new MdsGridViewDapperModel();
            conditionsString = conditionsString.Trim();
            var finalWhereString = "";
            var parameters = new DynamicParameters();
            var andConditions = conditionsString.Split(new[] { 'α' }, StringSplitOptions.RemoveEmptyEntries).Select(q => q.Trim()).Distinct().ToList();
            var counter = 0;
            foreach (var andCondition in andConditions)
            {
                var orConditions = andCondition.Split(new[] { 'β' }, StringSplitOptions.RemoveEmptyEntries).Select(q => q.Trim()).Distinct().ToList();
                var orConditionString = "";
                var numberOfOrConditions = 0;
                foreach (var orCondition in orConditions)
                {
                    counter++;
                    var parameterName = $"@param{counter}";
                    var propertyName = Regex.Match(orCondition, "^[^=]+").Value;

                    #region پیدا کردن پراپرتی متناظر با این پراپرتی

                    aliasDictionary.TryGetValue(propertyName, out var aliasPropertyNames);
                    if (!string.IsNullOrWhiteSpace(aliasPropertyNames)) propertyName = aliasPropertyNames;

                    #endregion

                    var operatorString = Regex.Match(orCondition, "[^¤]+$").Value;
                    var valueString = Regex.Match(orCondition, "(?<==)[^¤]+").Value;
                    if (string.IsNullOrWhiteSpace(valueString)) continue;
                    int.TryParse(operatorString, out var operatorInt);
                    var operatorEnum = (OperatorEnum)operatorInt;
                    if (operatorEnum == OperatorEnum.None) continue;
                    var orMdsGridViewWhereModel = GetTsqlCondition(operatorEnum, valueString, parameterName, propertyName.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries));
                    if (!string.IsNullOrWhiteSpace(orConditionString) &&
                        !string.IsNullOrWhiteSpace(orMdsGridViewWhereModel.WhereString))
                    {
                        orConditionString = $"{orMdsGridViewWhereModel.WhereString} OR {orConditionString}";
                        parameters.AddDynamicParams(orMdsGridViewWhereModel.Params);
                        numberOfOrConditions++;
                    }
                    else if (!string.IsNullOrWhiteSpace(orMdsGridViewWhereModel.WhereString))
                    {
                        orConditionString = orMdsGridViewWhereModel.WhereString;
                        parameters.AddDynamicParams(orMdsGridViewWhereModel.Params);
                        numberOfOrConditions++;
                    }
                }
                if (!string.IsNullOrWhiteSpace(orConditionString) && !string.IsNullOrWhiteSpace(finalWhereString))
                {
                    if (numberOfOrConditions > 1)
                        finalWhereString = $"{finalWhereString} AND ({orConditionString})";
                    else
                        finalWhereString = $"{finalWhereString} AND {orConditionString}";
                }
                else if (!string.IsNullOrWhiteSpace(orConditionString))
                {
                    finalWhereString = orConditionString;
                }
            }
            return new MdsGridViewDapperModel
            {
                WhereString = string.IsNullOrWhiteSpace(finalWhereString) ? "" : $"WHERE {finalWhereString}",
                Params = parameters
            };
        }

        private static string GetSortDirectionTsqlString(SortDirectionEnum sortDirectionEnum)
        {
            switch (sortDirectionEnum)
            {
                case SortDirectionEnum.Ascending:
                    return " ASC";

                case SortDirectionEnum.Descending:
                    return " DESC";
            }
            return "";
        }
        private static string GetSortString(string conditionString, KeyValuePair<string, SortDirectionEnum> defaultSort, IReadOnlyDictionary<string, string> aliasDictionary)
        {
            if (string.IsNullOrWhiteSpace(conditionString))
            {
                var defaultSortPropertyName = defaultSort.Key;
                aliasDictionary.TryGetValue(defaultSortPropertyName, out var aliasPropertyNames);
                if (!string.IsNullOrWhiteSpace(aliasPropertyNames)) defaultSortPropertyName = aliasPropertyNames.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                return $"{defaultSortPropertyName} {GetSortDirectionTsqlString(defaultSort.Value)}";
            }
            var sortConditions = "";
            var sortItemsStringParts = conditionString.Split(new[] { 'β' }, StringSplitOptions.RemoveEmptyEntries).Select(q => q.Trim()).Distinct().ToList();
            foreach (var sortItemString in sortItemsStringParts)
            {
                var sortItemParts = sortItemString.Split(new[] { '¤' }, StringSplitOptions.RemoveEmptyEntries).Select(q => q.Trim()).Distinct().ToList();
                if (sortItemParts.Count < 2) continue;
                var propertyName = sortItemParts[0];

                #region پیدا کردن پراپرتی متناظر با این پراپرتی

                aliasDictionary.TryGetValue(propertyName, out var aliasPropertyNames);
                if (!string.IsNullOrWhiteSpace(aliasPropertyNames)) propertyName = aliasPropertyNames.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();

                #endregion

                var sortType = sortItemParts[1];
                if (!int.TryParse(sortType, out var sortingTypeNumber)) continue;
                if (sortingTypeNumber > (int)SortDirectionEnum.Descending || sortingTypeNumber <= 0) continue;
                if (string.IsNullOrWhiteSpace(sortConditions))
                    sortConditions = propertyName;
                else
                    sortConditions += $", {propertyName}";
                sortConditions += GetSortDirectionTsqlString((SortDirectionEnum)sortingTypeNumber);
            }
            return sortConditions;
        }
        public static Dictionary<object, object> GetDictionaries<T>(T model)
        {
            var typeOfT = typeof(T);
            var properties = typeOfT.GetProperties();
            var dictionary = new Dictionary<object, object>();
            foreach (var propertyInfo in properties)
            {
                var value = propertyInfo.GetValue(model);
                var propertyName = propertyInfo.Name;
                if (value == null)
                {
                    value = string.Empty;
                    dictionary.Add(propertyName, value);
                    continue;
                }
                if (DateTimeTypes.Contains(propertyInfo.PropertyType)) // DateTime
                {
                    var value1 = value as DateTime?;
                    if (!value1.HasValue || value1.Value <= DateTime.MinValue)
                        value = string.Empty;
                }
                dictionary.Add(propertyName, value);
            }
            return dictionary;
        }
        public static MdsGridViewDapperModel GetMdsGridViewDapperModel(TsqlGridViewConditions model, KeyValuePair<string, SortDirectionEnum> defaultSort, Dictionary<string, string> aliasDictionary)
        {
            if (model == null) model = new TsqlGridViewConditions();
            model.Page = model.Page <= 0 ? 1 : model.Page;
            var whereConditions = GetMdsGridViewWhereConditions(model.WhereConditions, aliasDictionary);
            var parameters = new DynamicParameters();
            if (!string.IsNullOrWhiteSpace(whereConditions.WhereString))
                parameters.AddDynamicParams(whereConditions.Params);
            parameters.Add("firstRowNumber", GetFirstRowNumber(model.Page, model.ItemsPerPage));
            parameters.Add("lastRowNumber", GetLastRowNumber(model.Page, model.ItemsPerPage));
            whereConditions.Params = parameters;
            whereConditions.SortString = GetSortString(model.SortConditions, defaultSort, aliasDictionary);
            return whereConditions;
        }

        private class TsqlCondition
        {
            public string Tsql { get; set; }
            public object ParameterValue { get; set; }
        }
    }
}