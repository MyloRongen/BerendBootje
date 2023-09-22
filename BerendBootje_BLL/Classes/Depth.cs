using BerendBootje_BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerendBootje_BLL.Classes
{
    public class Depth
    {
        private readonly List<Container> containers;

        public Depth()
        {
            containers = new List<Container>();
        }

        public void AddContainer(Container container)
        {
            containers.Add(container);
        }

        public void AddContainerBelowOtherContainers(Container container)
        {
            containers.Insert(0, container);
        }

        public int GetDepthCount()
        {
            return containers.Count;
        }

        public List<Container> GetContainers()
        {
            return containers;
        }

        public Container? GetLastContainer()
        {
            return containers.LastOrDefault();
        }

        public bool ContainsValuableOrValuableCoolable()
        {
            foreach (Container container in containers)
            {
                if (container?.ContainerValueType is ContainerValueType.Valuable or ContainerValueType.ValuableCoolable)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
