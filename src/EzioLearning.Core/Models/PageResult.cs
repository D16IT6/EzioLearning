using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzioLearning.Core.Models
{
    public class PageResult<T> : PageResultBase where T : class
    {
        public List<T> Data { get; set; } = [];
    }
}
