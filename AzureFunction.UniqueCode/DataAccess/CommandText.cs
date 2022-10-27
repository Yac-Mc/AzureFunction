using AzureFunction.UniqueCode.Domain;
using AzureFunction.UniqueCode.Entities.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureFunction.UniqueCode.DataAccess
{
    public class CommandText : ICommandText
    {
        public string SelectAll(string columnsNames, string tableName) => $"SELECT {columnsNames} FROM {tableName}";
        public string SelectAll(string columnsNames, string tableName, List<Filter> filters) => $"SELECT {columnsNames} FROM {tableName} {GenerateFilters(filters, tableName)}";

        #region Methods privates
        private protected static string GenerateFilters(List<Filter> filters, string tableName)
        {
            StringBuilder where = new StringBuilder();
            if (filters != null && filters.Any())
            {
                where.Append("WHERE ");
                foreach (var filter in filters.OrderBy(x => x.Order).ToList())
                {
                    string nextCondition = "";
                    if (filter.NextCondition != FilterNextCondition.Empty)
                    {
                        nextCondition = (filter.NextCondition == FilterNextCondition.AND) ? "AND" : "OR";
                    }
                    if (filter.Operator == FilterOperator.In || filter.Operator == FilterOperator.NotIn || filter.Operator == FilterOperator.Between)
                    {
                        string convertValueToString = (string)filter.Value;
                        filter.Value = string.Join((filter.Operator == FilterOperator.Between) ? " AND " : ",", 
                            convertValueToString.Split(',').Select(x => filter.Operator_IN_Between_ConvertType == FilterOperator_IN_Between_Type.String ? $"'{x.Trim()}'" : $"{x.Trim()}").ToList());
                    }
                    else
                    {
                        filter.Value = (filter.Value.GetType().ToString().Contains("String")) ? $"'{filter.Value}'" : filter.Value;

                    }
                    switch (filter.Operator)
                    {
                        case FilterOperator.Equals:
                            where.Append($"({tableName}.{filter.Column} = {filter.Value}) {nextCondition} ");
                            break;
                        case FilterOperator.Different:
                            where.Append($"({tableName}.{filter.Column} <> {filter.Value}) {nextCondition} ");
                            break;
                        case FilterOperator.Mayor:
                            where.Append($"({tableName}.{filter.Column} > {filter.Value}) {nextCondition} ");
                            break;
                        case FilterOperator.Minor:
                            where.Append($"({tableName}.{filter.Column} < {filter.Value}) {nextCondition} ");
                            break;
                        case FilterOperator.Like:
                            where.Append($"({tableName}.{filter.Column} LIKE '%{filter.Value}%') {nextCondition} ");
                            break;
                        case FilterOperator.Between:
                            where.Append($"({tableName}.{filter.Column} BETWEEN '{filter.Value}') {nextCondition} ");
                            break;
                        case FilterOperator.In:
                            where.Append($"({tableName}.{filter.Column} IN ({filter.Value})) {nextCondition} ");
                            break;
                        case FilterOperator.NotIn:
                            where.Append($"({tableName}.{filter.Column} NOT IN ({filter.Value})) {nextCondition} ");
                            break;
                    }
                }
            }

            return $"{where}";
        }

        #endregion
    }
}
