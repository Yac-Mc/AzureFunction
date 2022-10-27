namespace AzureFunction.UniqueCode.Entities.Queries
{
    public class Filter
    {
        /// <summary>
        /// Specific order on Where
        /// </summary>
        public int Order { get; set; } = 1;

        /// <summary>
        /// Column Name from the table
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// Unary operator to validate the condition (equals, different, mayor, minor, like, between, in...)
        /// </summary>
        public FilterOperator Operator { get; set; } = FilterOperator.Equals;

        /// <summary>
        /// If the Operator variable is equals to 'IN, NOT IN or Between' then specify the type to which you want to convert the value (string or int)
        /// </summary>
        public FilterOperator_IN_Between_Type Operator_IN_Between_ConvertType { get; set; } = FilterOperator_IN_Between_Type.String;

        /// <summary>
        /// Exact value to place (If the Operator variable is equal to 'IN, NOT IN or Between' separate it by commas)
        /// </summary>
        public dynamic Value { get; set; }

        /// <summary>
        /// Condition to validate the filter, (AND, OR)
        /// </summary>
        public FilterNextCondition NextCondition { get; set; } = FilterNextCondition.Empty;
    }

    public enum FilterOperator
    {
        Equals = 1,
        Different = 2,
        Mayor = 3,
        Minor = 4,
        Like = 5,
        Between = 6,
        In = 7,
        NotIn = 8
    }

    public enum FilterOperator_IN_Between_Type
    {
        String = 1,
        Int = 2
    }

    public enum FilterNextCondition
    {
        Empty = 0,
        AND = 1,
        OR = 2
    }
}
