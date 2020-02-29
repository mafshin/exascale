using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.Common
{
    public class SampleCustomerInformation
    {
        public int CustomerId { get; set; }
        public object Birthdate { get; internal set; }
        public object Job { get; internal set; }
        public object Title { get; internal set; }
        public object Address { get; internal set; }
    }
}
