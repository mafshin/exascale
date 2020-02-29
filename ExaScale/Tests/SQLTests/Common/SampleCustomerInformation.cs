using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.Common
{
    public class SampleCustomerInformation
    {
        public int CustomerId { get; set; }
        public DateTime Birthdate { get; internal set; }
        public string Job { get; internal set; }
        public string Title { get; internal set; }
        public string Address { get; internal set; }
    }
}
