using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Models
{
    public class GetIsCustomerDetailsFoundResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public bool IsFound { get; set; }
    }
}
