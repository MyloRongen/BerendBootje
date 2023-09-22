using BerendBootje_BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerendBootje_BLL.Classes
{
    public class Container
    {
        public double Weight { get; set; }
        public ContainerValueType ContainerValueType { get; set; }

        public Container(double weight, ContainerValueType containerValueType) 
        { 
            Weight = weight;
            ContainerValueType = containerValueType;
        }
    }
}
