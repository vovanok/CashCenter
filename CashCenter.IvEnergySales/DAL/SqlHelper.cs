using System;

namespace CashCenter.IvEnergySales.DAL
{
    public static class SqlHelper
    {
        public const string CUSTOMER_ID = "ID";
        public const string CUSTOMER_NAME = "NAME";
        public const string CUSTOMER_FLAT = "FLAT";
        public const string CUSTOMER_BUILDING_NUMBER = "BUILDING_NUMBER";
        public const string CUSTOMER_STREET_NAME = "STREET_NAME";
        public const string CUSTOMER_LOCALITY_NAME = "LOCALITY_NAME";

        public const string CUSTOMER_COUNTERS_END_DAY_VALUE = "END_DAY_VALUE";
        public const string CUSTOMER_COUNTERS_END_NIGHT_VALUE = "END_NIGHT_VALUE";
        public const string CUSTOMER_COUNTERS_IS_TWO_TARIFF = "IS_TWO_TARIFF";

        public const string REASON_ID = "ID";
        public const string REASON_NAME = "NAME";
        public const string REASON_CANPAY = "CANPAY";

        private static readonly string SQL_GET_REASONS_FORMAT = $"select {REASON_ID}, {REASON_NAME}, {REASON_CANPAY} from reason";

        private static readonly string SQL_GET_CUSTOMER_FORMAT =
            $@"select * from
                   (select 
                          customer.id {CUSTOMER_ID}, 
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
                   where customer.id = {{0}} and customer.aggr$state_id <> 4) finded_customer
               left join
                   (select {{0}} ID, {CUSTOMER_COUNTERS_END_DAY_VALUE}, {CUSTOMER_COUNTERS_END_NIGHT_VALUE}, {CUSTOMER_COUNTERS_IS_TWO_TARIFF}
                       from R090$GET_COUNTER_VALUES({{0}}, '{{1}}', '{{2}}')) counter_values
               on finded_customer.ID = counter_values.ID";
        
        public static string GetSqlForGetCustomer(int customerId, DateTime startDate, DateTime endDate)
        {
            return string.Format(SQL_GET_CUSTOMER_FORMAT, customerId, startDate.ToShortDateString(), endDate.ToShortDateString());
        }

        public static string GetSqlForGetReasons()
        {
            return SQL_GET_REASONS_FORMAT;
        }
    }
}
