using BerendBootje_BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerendBootje_BLL.Classes
{
    public class CargoManagement
    {
        private readonly List<Ship> ships;
        private readonly int numRows = 6;
        private readonly int numContainersPerRow = 3;

        private readonly int numNormal = 40;
        private readonly int numCoolable = 8;
        private readonly int numValuableCoolable = 2;
        private readonly int numValuable = 10;

        public CargoManagement() 
        {
            ships = new List<Ship>();
        }

        public void CreateShip()
        {
            Ship ship = new(numRows, numContainersPerRow);

            for (int i = 0; i < numCoolable; i++)
            {
                ship.AddContainerToShip(new Container(30, ContainerValueType.Coolable));
            }

            for (int i = 0; i < numNormal; i++)
            {
                ship.AddContainerToShip(new Container(30, ContainerValueType.Normal));
            }

            for (int i = 0; i < numValuableCoolable; i++)
            {
                ship.AddContainerToShip(new Container(30, ContainerValueType.ValuableCoolable));
            }

            for (int i = 0; i < numValuable; i++)
            {
                ship.AddContainerToShip(new Container(30, ContainerValueType.Valuable));
            }

            ships.Add(ship);
        }

        public void SortContainersInShips()
        {
            foreach (Ship ship in ships)
            {
                ship.SortContainersToShip();
            }

            CreateOutputContainers();
        }

        private void CreateOutputContainers()
        {
            List<Row> Rows = ships[0].Rows;
            bool isFirstStack = true;
            bool isFirstDepth = true;
            bool isFirstContainer = true;

            string stacks = "";
            string weights = "";

            for (int i = 0; i < Rows[0].GetContainersWithDepthCount(); i++)
            {
                stacks += isFirstStack ? "" : "/";
                weights += isFirstStack ? "" : "/";
                isFirstStack = false;
                isFirstDepth = true;
                isFirstContainer = true;

                foreach (Row row in Rows)
                {
                    stacks += isFirstDepth ? "" : ",";
                    weights += isFirstDepth ? "" : ",";
                    isFirstDepth = false;
                    isFirstContainer = true;

                    foreach (Container container in row.GetContainersWithDepth()[i].GetContainers())
                    {
                        stacks += isFirstContainer ? "" : "-";
                        weights += isFirstContainer ? "" : "-";
                        isFirstContainer = false;

                        ContainerValueType valueType = container.ContainerValueType;
                        int enumValue = (int)valueType;

                        stacks += (enumValue + 1).ToString();
                        weights += container.Weight.ToString();
                    }
                }
            }

            string directory = $"https://i872272.luna.fhict.nl/ContainerVisualizer/index.html?length={numRows}&width={numContainersPerRow}&stacks={stacks}&weights={weights}";
            Console.WriteLine(directory);
        }
    }
}
