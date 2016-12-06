namespace CashCenter.IvEnergySales.DataModel
{
    public class Customer
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public string Flat { get; private set; }

        public string BuildingNumber { get; private set; }

        public string StreetName { get; private set; }

        public string LocalityName { get; private set; }

        public Customer(int id, string name, string flat,
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
