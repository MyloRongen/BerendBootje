using BerendBootje_BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BerendBootje_BLL.Classes
{
    public class Ship
    {
        private double minWeight;
        private double maxWeight;

        private readonly int _numRows;
        private readonly int _numContainersPerRow;

        private const double MinimumWeightPerContainer = 4;
        private const double MaximumWeightPerContainer = 30;

        public List<Container> containers;
        public List<Row> Rows { get; set; }

        public Ship(int numRows, int numContainersPerRow)
        {
            _numRows = numRows;
            _numContainersPerRow = numContainersPerRow;

            Rows = new List<Row>(numRows);
            InitializeRows(numRows);

            CalculateMaximumWeightShip();
            containers = new List<Container>();
        }

        private void InitializeRows(int numRows)
        {
            for (int i = 0; i < numRows; i++)
            {
                AddRow(numRows);
            }
        }

        public void AddRow(int numRows)
        {
            Rows.Add(new Row(numRows, _numContainersPerRow));
        }

        private void CalculateMaximumWeightShip()
        {
            maxWeight = (_numRows * _numContainersPerRow) * 150;
            minWeight = maxWeight / 2;
        }

        public void SortContainersToShip()
        {
            CategorizeContainers(containers, 
                out List<Container> valuableContainers, 
                out List<Container> cooledContainers, 
                out List<Container> normalContainers, 
                out List<Container> valuableCooledContainers
            );

            CheckValuableContainersStatus(valuableContainers, valuableCooledContainers);

            SortCooledContainers(cooledContainers);
            SortValuableCooledContainers(valuableCooledContainers);
            SortValuableContainers(valuableContainers);
            SortNormalContainers(normalContainers);

            CheckWeightStatus();
        }

        private void CheckValuableContainersStatus(List<Container> valuableContainers, List<Container> valuableCooledContainers)
        {
            int rowsEmpty = (int)Math.Floor((double)_numRows / 2) - 1;

            if ((_numContainersPerRow * _numRows) - (rowsEmpty * _numContainersPerRow) < (valuableContainers.Count + valuableCooledContainers.Count))
            {
                Console.WriteLine("Too many valuable containers!");
            }
        }

        private void SortValuableCooledContainers(List<Container> valuableCooledContainers)
        {
            if (_numContainersPerRow < valuableCooledContainers.Count)
            {
                Console.WriteLine("Too many valuableCooledContainers!");
            }

            foreach (Container valuableCooledContainer in valuableCooledContainers)
            {
                Rows[0].AddValuableCooledContainerToRow(valuableCooledContainer);
            }
        }

        private void SortValuableContainers(List<Container> valuableContainers)
        {
            int numRows = Rows.Count;
            int currentRowIdx = 0;

            int valuableContainersPlacedInCurrentRow = 0;

            foreach (Container valuableContainer in valuableContainers)
            {
                bool isContainerAdded = AddValuableContainerToCurrentRowIfNeeded(currentRowIdx, valuableContainers, ref valuableContainersPlacedInCurrentRow);

                if (!isContainerAdded)
                {
                    UpdateCurrentRowIdx(numRows, ref currentRowIdx);
                    AddValuableContainerToCurrentRowIfNeeded(currentRowIdx, valuableContainers, ref valuableContainersPlacedInCurrentRow);
                }

                if (valuableContainersPlacedInCurrentRow < _numContainersPerRow)
                {
                    continue;
                }

                UpdateCurrentRowIdx(numRows, ref currentRowIdx);

                valuableContainersPlacedInCurrentRow = 0;
            }
        }

        private bool AddValuableContainerToCurrentRowIfNeeded(int currentRowIdx, List<Container> valuableContainers, ref int valuableContainersPlacedInCurrentRow)
        {
            if (!HasValuableContainersBeneathRow(currentRowIdx))
            {
                Rows[currentRowIdx].AddValuableContainerToRow(valuableContainers[valuableContainersPlacedInCurrentRow]);
                valuableContainersPlacedInCurrentRow++;

                return true;
            }

            return false;
        }

        private bool HasValuableContainersBeneathRow(int currentRowIdx)
        {
            List<Depth> depths = Rows[currentRowIdx].GetContainersWithDepth();
            int valuableContainerBeneath = 0;

            foreach (Depth depth in depths)
            {
                if (depth.GetDepthCount() != 0)
                {
                    if (depth.GetLastContainer().ContainerValueType == ContainerValueType.Valuable || depth.GetLastContainer().ContainerValueType == ContainerValueType.ValuableCoolable)
                    {
                        valuableContainerBeneath++;
                    }
                }
            }

            if (valuableContainerBeneath == _numContainersPerRow)
            {
                return true;
            }

            return false;
        }

        private static void UpdateCurrentRowIdx(int numRows, ref int currentRowIdx)
        {
            currentRowIdx++;

            if (numRows > 2 && currentRowIdx >= 2 && currentRowIdx != numRows - 1)
            {
                currentRowIdx += 1;
            }

            if (currentRowIdx >= numRows)
            {
                currentRowIdx %= numRows;
            }        
        }

        private void SortCooledContainers(List<Container> cooledContainers)
        {
            foreach (Container cooledContainer in cooledContainers)
            {
                Rows[0].AddCooledContainerToRow(cooledContainer);
            }
        }

        private void SortNormalContainers(List<Container> normalContainers)
        {
            foreach (Container normalContainer in normalContainers)
            {
                IEnumerable<Row> rowsWithValuables = GetRowsWithValuables();

                int lowestCount = Rows.Min(row => CalculateMinimumCountForRow(row));
                int lowestCountRow = GetRowsWithValuables().Max(row => CalculateMinimumCountForRow(row));

                PlaceNormalContainer(normalContainer, lowestCount, lowestCountRow);
            }
        }

        private void PlaceNormalContainer(Container normalContainer, int lowestCount, int lowestCountRow)
        {
            IEnumerable<Row> rowsWithValuables = GetRowsWithValuables();

            if (ShouldPrioritizeValuableRows(rowsWithValuables, lowestCount, lowestCountRow))
            {
                Row rowWithLowestCount = rowsWithValuables
                        .OrderBy(row => CalculateMinimumCountForRow(row))
                        .First();
                rowWithLowestCount.AddNormalContainerToRow(normalContainer);
            }
            else
            {
                Row rowWithLowestCount = FindRowWithLowestCount(Rows, lowestCount);
                rowWithLowestCount.AddNormalContainerToRow(normalContainer);
            }
        }

        private static bool ShouldPrioritizeValuableRows(IEnumerable<Row> rowsWithValuables, int lowestCount, int lowestCountRow)
        {
            return rowsWithValuables.Any() && lowestCountRow == lowestCount + 2;
        }

        private IEnumerable<Row> GetRowsWithValuables()
        {
            return Rows.Where(row => row.ContainsValuableOrValuableCoolable());
        }

        private static int CalculateMinimumCountForRow(Row row)
        {
            int rowContainerCount = row.GetContainerCount();
            int containerDepthCount = row.GetContainersWithDepthCount();

            return (int)Math.Floor((double)rowContainerCount / containerDepthCount);
        }

        private static Row FindRowWithLowestCount(List<Row> rows, int lowestCount)
        {
            IEnumerable<Row> matchingRows = rows.Select(row => new
            {
                Row = row,
                MinimumCount = CalculateMinimumCountForRow(row)
            }).Where(item => item.MinimumCount == lowestCount).Select(item => item.Row);

            return matchingRows.First();
        }

        private static void CategorizeContainers(List<Container> containers, out List<Container> valuableContainers, out List<Container> cooledContainers, out List<Container> normalContainers, out List<Container> valuableCooledContainers)
        {
            valuableContainers = new List<Container>();
            cooledContainers = new List<Container>();
            normalContainers = new List<Container>();
            valuableCooledContainers = new List<Container>();

            foreach (Container container in containers)
            {
                switch (container.ContainerValueType)
                {
                    case ContainerValueType.Valuable:
                        valuableContainers.Add(container);
                        break;
                    case ContainerValueType.Coolable:
                        cooledContainers.Add(container);
                        break;
                    case ContainerValueType.ValuableCoolable:
                        valuableCooledContainers.Add(container);
                        break;
                    default:
                        normalContainers.Add(container);
                        break;
                }
            }
        }

        public void AddContainerToShip(Container container)
        {
            if (HasRightAmountOfWeight(container))
            {
                containers.Add(container);
            }
        }

        private static bool HasRightAmountOfWeight(Container container)
        {
            if (container.Weight >= MinimumWeightPerContainer && container.Weight <= MaximumWeightPerContainer)
            {
                return true;
            }

            return false;
        }

        private void CheckWeightStatus()
        {
            if (CalculateTotalWeightContainersInShip() >= minWeight)
            {
                Console.WriteLine("has the correct weight");
            }
            else
            {
                Console.WriteLine("Doesn't have the required weight of 50% from the ship");
            }
        }

        private double CalculateTotalWeightContainersInShip()
        {
            double totalWeightContainers = 0;

            foreach (Container container in containers)
            {
                totalWeightContainers += container.Weight;
            }

            return totalWeightContainers;
        }
    }
}
