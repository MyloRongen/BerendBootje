using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerendBootje_BLL.Classes
{
    public class Row
    {
        private readonly List<Depth> containersWithDepth;

        public Row(int numRows, int numContainers)
        {
            containersWithDepth = new(numContainers);

            for (int i = 0; i < numContainers; i++)
            {
                containersWithDepth.Add(new Depth());
            }       
        }

        public void AddCooledContainerToRow(Container container)
        {
            AddContainerToRow(container);
        }

        public void AddValuableCooledContainerToRow(Container container)
        {
            AddContainerToRow(container);
        }

        public void AddValuableContainerToRow(Container container)
        {
            Depth? minDepth = null;
            int minDepthCount = int.MaxValue;

            foreach (Depth depth in containersWithDepth)
            {
                if (IsDepthIsEmpty(depth))
                {
                    minDepth = depth;
                    break;
                }

                int depthCount = depth.GetDepthCount();
                if (depthCount < minDepthCount && !depth.ContainsValuableOrValuableCoolable())
                {
                    minDepth = depth;
                    minDepthCount = depthCount;
                }
            }

            minDepth?.AddContainer(container);
        }

        private static bool IsDepthIsEmpty(Depth depth)
        {
            if (depth.GetDepthCount() == 0)
            {
                return true;
            }
            return false;
        }

        private void AddContainerToRow(Container container)
        {
            Depth? minDepth = containersWithDepth.FirstOrDefault(d => d.GetDepthCount() == 0);
            minDepth ??= containersWithDepth.OrderBy(d => d.GetDepthCount()).FirstOrDefault();

            minDepth?.AddContainer(container);
        }

        public void AddNormalContainerToRow(Container container)
        {
            Depth? minDepth = containersWithDepth.FirstOrDefault(d => d.GetDepthCount() == 0);
            minDepth ??= containersWithDepth.OrderBy(d => d.GetDepthCount()).FirstOrDefault();

            minDepth?.AddContainerBelowOtherContainers(container);
        }

        public int GetContainersWithDepthCount()
        {
            return containersWithDepth.Count;
        }

        public int GetContainerCount()
        {
            int totalContainerCount = 0;

            foreach (Depth depth in containersWithDepth)
            {
                totalContainerCount += depth.GetDepthCount();
            }

            return totalContainerCount;
        }

        public List<Depth> GetContainersWithDepth()
        {
            return containersWithDepth;
        }

        public bool ContainsValuableOrValuableCoolable()
        {
            foreach (Depth depth in containersWithDepth)
            {
                if (IsDepthIsEmpty(depth))
                {
                    break;
                }

                if (depth.ContainsValuableOrValuableCoolable())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
