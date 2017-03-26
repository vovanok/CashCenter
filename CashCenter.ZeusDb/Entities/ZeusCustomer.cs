using System.Linq;

namespace CashCenter.ZeusDb.Entities
{
    public class ZeusCustomer
    {
        public int Id { get; private set; }

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

        public ZeusCustomer(int id, string departamentCode, string name, string flat,
            string buildingNumber, string streetName, string localityName)
        {
            Id = id;
            Name = name;
            Flat = flat;
            BuildingNumber = buildingNumber;
            StreetName = streetName;
            LocalityName = localityName;
        }
    }
}
