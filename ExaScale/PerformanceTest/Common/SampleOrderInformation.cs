using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest.Common
{
    public class SampleOrderInformation
    {
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderEntryDate { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
    }
}
