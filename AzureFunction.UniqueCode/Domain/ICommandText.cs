using AzureFunction.UniqueCode.Entities.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.UniqueCode.Domain
{
    public interface ICommandText
    {
        /// <summary>
        /// Query to get all data from a table, select (columnNames) From (tableName).
        /// </summary>
        /// <param name="columnsNames"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        string SelectAll(string columnsNames, string tableName);

        /// <summary>
        /// Query to get all the data in a table using filters with Where, Select (columnsNames) From (tableName) Where (Filters).
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filters"></param>
        /// <param name="columnsNames"></param>
        /// <returns></returns>
        string SelectAll(string columnsNames, string tableName, List<Filter> filters);
    }
}
