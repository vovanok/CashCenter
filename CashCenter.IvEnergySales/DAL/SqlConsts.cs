namespace CashCenter.IvEnergySales.DAL
{
    public static class SqlConsts
    {
        public const string CUSTOMER_ID = "ID";
        public const string CUSTOMER_NAME = "NAME";
        public const string CUSTOMER_FLAT = "FLAT";
        public const string CUSTOMER_BUILDING_NUMBER = "BUILDING_NUMBER";
        public const string CUSTOMER_STREET_NAME = "STREET_NAME";
        public const string CUSTOMER_LOCALITY_NAME = "LOCALITY_NAME";

        public static readonly string SQL_GET_CUSTOMER_FORMAT =
            $@"select customer.id {CUSTOMER_ID},
                      customer.name {CUSTOMER_NAME},
                      customer.flat {CUSTOMER_FLAT},
                      customer.aggr$state_id,
                      customer_building.building_number {CUSTOMER_BUILDING_NUMBER},
                      customer_building.street_name {CUSTOMER_STREET_NAME},
                      customer_building.locality_name {CUSTOMER_LOCALITY_NAME}
               from customer
                   left join
                       (select building.id ID,
                           building.number BUILDING_NUMBER,
                           street.name STREET_NAME,
                           locality.name LOCALITY_NAME
                        from building
                            inner join street on building.street_id = street.id
                            inner join locality on building.locality_id = locality.id) customer_building
                   on customer.building_id = customer_building.ID
               where customer.id = {{0}} and customer.aggr$state_id <> 4";
    }
}
