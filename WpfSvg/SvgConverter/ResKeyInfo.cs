using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSvg.SvgConverter {
    internal class ResKeyInfo {
        public string Name { get; set; }
        public string XamlName { get; set; }
        public string Prefix { get; set; }
        public bool UseComponentResKeys { get; set; }
        public string NameSpace { get; set; }
        public string NameSpaceName { get; set; }
    }
}
