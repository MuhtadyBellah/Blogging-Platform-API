using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Default_Project.Cores.Specifications
{
    public enum SortOptions
    {
        [EnumMember(Value = "IdDesc")]
        IdDesc,
        [EnumMember(Value = "Category")]
        Category,
        [EnumMember(Value = "CategoryDesc")]
        CategoryDesc,
        [EnumMember(Value = "createdAt")]
        Created,
        [EnumMember(Value = "Modified")]
        Modified
    }
}
