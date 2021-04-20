using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYL.Data.Models
{
    public class ModelBase
    {
        [PrimaryKey]
        [AutoIncrement]
        public string id { get; set; }
    }
}
