using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Models
{
    public class PaymentGetwayRequest
    {
        public int CartID { get; set; }
        public string Upiid { get; set; }
        public string PaymentType { get; set; }
        public string CardNo { get; set; }
    }
}
