namespace PSoC.ManagementService.Models
{
    /// <summary>
    /// Class that encapsulates most parameters consumed by DataTables plugin
    /// </summary>
    public class DataTablePageRequestModel
    {
        /// <summary>
        /// Display start point/first record, in the current data set, to be shown
        /// </summary>
        public int DisplayStart { get; set; }

        /// <summary>
        /// Number of records (page size) that the table can display in the current draw. It is expected that
        /// the number of recordsreturned will be equal to this number, unless the server has fewer records to return.
        /// </summary>
        public int DisplayLength { get; set; }

        /// <summary>
        /// Optional - this is a string of column names, comma separated (used in combination with sName) which will allow DataTables to reorder data on the client-side if required for display.
        /// Note that the number of column names returned must exactly match the number of columns in the table. For a more flexible JSON format, please consider using mDataProp.
        /// </summary>
        public string ColumnNames { get; set; }

        /// <summary>
        /// Number of columns being displayed (useful for getting individual column search info)
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Number of columns to sort on
        /// </summary>
        public int? SortingCols { get; set; }

        /// <summary>
        /// Global search field
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Information for DataTables to use for rendering.
        /// </summary>
        public int Echo { get; set; }
    }
}