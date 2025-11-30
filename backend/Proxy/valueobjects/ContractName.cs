using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy.valueobjects
{
    public class ContractName
    {
        public string Name { get; set; }
        public ContractName() { }

        public void init(string name) {
            this.Name = name;
        }
    }
}
