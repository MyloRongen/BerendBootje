using BerendBootje_BLL.Classes;

namespace BerendBootje
{
    public class Program
    {
        static void Main(string[] args)
        {
            /*            Console.WriteLine("Hello, World!");*/
            CargoManagement cargoManagement = new();
            cargoManagement.CreateShip();

            cargoManagement.SortContainersInShips();
        }
    }
}