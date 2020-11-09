//-----> Contributed By- Abhishek Tiwari (849729)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClaimsMicroservice.Models
{
    public class ProviderPolicy
    {
        public int HospitalID { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddress { get; set; }
        public int PolicyID { get; set; }
        public string Location { get; set; }
    }
}
