using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Models
{
    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DataTableSearch? Search { get; set; }
        public List<DataTableOrder>? Order { get; set; }
        public List<DataTableColumn>? Columns { get; set; }

        public string GetSortColumn()
        {
            if (Order == null || Order.Count == 0 ||
                Columns == null || Columns.Count == 0)
                return "Username";

            var colIndex = Order[0].Column;
            return Columns[colIndex].Data switch
            {
                "username" => "Username",
                "email" => "Email",
                "role" => "Role",
                "createdAt" => "CreatedAt",
                _ => "Username"
            };
        }

        public string GetSortDir()
        {
            return Order?.FirstOrDefault()?.Dir?.ToLower() == "desc" ? "desc" : "asc";
        }
    }

    public class DataTableSearch { public string? Value { get; set; } }
    public class DataTableOrder { public int Column { get; set; } public string Dir { get; set; } }
    public class DataTableColumn { public string Data { get; set; } }

}
