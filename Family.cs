using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureKeyVaultDemo
{
    public class Family
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string FamilyName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Job { get; set; }

        public Address Addresses { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class Address
    {
        public string Id { get; set; }
        public string StreetName { get; set; }

        public string HouseName { get; set; }
    }
}
