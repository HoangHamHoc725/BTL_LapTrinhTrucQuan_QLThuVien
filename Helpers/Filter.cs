using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.Helpers
{
    public class Filter
    {
        public string DisplayName { get; set; }
        public string ColumnName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }

        public Filter(string displayName, string columnName, string value)
        {
            DisplayName = displayName;
            ColumnName = columnName;
            Value = value;
        }

        public override string ToString()
        {
            return $"{DisplayName} {Operator} {Value}";
        }
    }
}
