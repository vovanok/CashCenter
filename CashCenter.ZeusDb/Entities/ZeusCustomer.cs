using System.Linq;

namespace CashCenter.ZeusDb.Entities
{
    public class ZeusCustomer
    {
        public int Number { get; private set; }

        public string DepartamentCode { get; private set; }

        public string Name { get; private set; }

        public string Flat { get; private set; }

        public string BuildingNumber { get; private set; }

        public string StreetName { get; private set; }

        public string LocalityName { get; private set; }

        public string Address
        {
            get
            {
                var addressComponents = new[] { LocalityName, StreetName, BuildingNumber, Flat };
                return string.Join(", ", addressComponents.Where(item => !string.IsNullOrEmpty(item)));
            }
        }

        public ZeusCustomer(int number, string departamentCode, string name, string flat,
            string buildingNumber, string streetName, string localityName)
        {
            Number = number;
            Name = name;
            Flat = flat;
            BuildingNumber = buildingNumber;
            StreetName = streetName;
            LocalityName = localityName;
        }
    }
}
