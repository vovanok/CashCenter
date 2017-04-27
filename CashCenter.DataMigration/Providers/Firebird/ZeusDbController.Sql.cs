namespace CashCenter.DataMigration.Providers.Firebird
{
    public partial class ZeusDbController
    {
        private static class Sql
        {
            public const string CUSTOMER_ID = "ID";
            public const string CUSTOMER_NAME = "NAME";
            public const string CUSTOMER_FLAT = "FLAT";
            public const string CUSTOMER_BUILDING_NUMBER = "BUILDING_NUMBER";
            public const string CUSTOMER_STREET_NAME = "STREET_NAME";
            public const string CUSTOMER_LOCALITY_NAME = "LOCALITY_NAME";

            public const string COUNTER_NAME = "COUNTER_NAME";
            public const string CUSTOMER_COUNTERS_END_DAY_VALUE = "END_DAY_VALUE";
            public const string CUSTOMER_COUNTERS_END_NIGHT_VALUE = "END_NIGHT_VALUE";
            public const string CUSTOMER_COUNTERS_IS_TWO_TARIFF = "IS_TWO_TARIFF";

            public const string REASON_ID = "ID";
            public const string REASON_NAME = "NAME";
            public const string REASON_CANPAY = "CANPAY";

            public const string CUSTOMER_COUNTER_ID = "CUSTOMER_COUNTER_ID";

            public const string PAY_JOURNAL_ID = "PAY_JOURNAL_ID";
            public const string PAY_JOURNAL_NAME = "PAY_JOURNAL_NAME";

            public const string PAYMENT_KIND_ID = "PAYMENT_KIND_ID";
            public const string PAYMENT_KIND_NAME = "PAYMENT_KIND_NAME";
            public const string PAYMENT_KIND_TYPE_ID = "PAYMENT_KIND_TYPE_ID";

            public const string END_BALANCE = "END_BALANCE";
            public const string PENALTY_BALANCE = "PENALTY_BALANCE";

            public const string PARAM_CUSTOMER_ID = "@CUSTOMER_ID";
            public const string PARAM_START_DATE = "@START_DATE";
            public const string PARAM_END_DATE = "@END_DATE";
            public const string PARAM_PAYMENT_KIND_ID = "@PAYMENT_KIND_ID";
            public const string PARAM_PAYMENT_KIND_NAME = "@PAYMENT_KIND_NAME";
            public const string PARAM_PAYMENT_TYPE_ID = "@PAYMENT_TYPE_ID";
            public const string PARAM_CREATE_DATE = "@CREATE_DATE";
            public const string PARAM_PAYMENT_COST = "@PAYMENT_COST";
            public const string PARAM_PAY_JOURNAL_NAME = "@PAY_JOURNAL_NAME";
            public const string PARAM_REASON_ID = "@REASON_ID";
            public const string PARAM_PENALTY_TOTAL = "@PENALTY_TOTAL";
            public const string PARAM_DESCRIPTION = "@DESCRIPTION";
            public const string PARAM_VALUE1 = "@VALUE1";
            public const string PARAM_VALUE2 = "@VALUE2";
            public const string PARAM_PAY_JOURNAL_ID = "@PAY_JOURNAL_ID";
            public const string PARAM_CUSTOMER_COUNTER_ID = "@CUSTOMER_COUNTER_ID";
            public const string PARAM_COUNTER_VALUES_ID = "@COUNTER_VALUES_ID";
            public const string PARAM_PAY_ID = "@PAY_ID";
            public const string PARAM_DAY_ENCODING = "@DAY_ENCODING";
            public const string PARAM_PENALTY_VALUE = "@PENALTY_VALUE";
            public const string PARAM_METERS_ID = "@METERS_ID";

            public const string WAREHOUSE_ID = "ID";
            public const string WAREHOUSE_CODE = "CODE";
            public const string WAREHOUSE_NAME = "NAME";
            public const string WAREHOUSE_QUANTITY = "QUANTITY";
            public const string WAREHOUSE_UNITNAME = "UNIT_NAME";
            public const string WAREHOUSE_BARCODE = "BARCODE";

            public const string WAREHOUSE_CATEGORY_ID = "ID";
            public const string WAREHOUSE_CATEGORY_CODE = "CODE";
            public const string WAREHOUSE_CATEGORY_NAME = "NAME";
            public const string WAREHOUSE_CATEGORY_IS_DEFAULT = "IS_DEFAULT";
            public const string WAREHOUSE_CATEGORY_IS_WHOLESALE = "IS_WHOLESALE";

            public const string WAREHOUSE_PRICE_ID = "ID";
            public const string WAREHOUSE_PRICE_WAREHOUSE_ID = "WAREHOUSE_ID";
            public const string WAREHOUSE_PRICE_WAREHOUSE_CATEGORY_ID = "WAREHOUSE_CATEGORY_ID";
            public const string WAREHOUSE_PRICE_ENTRY_DATE = "ENTRY_DATE";
            public const string WAREHOUSE_PRICE_PRICE_VALUE = "PRICE_VALUE";

            public static readonly string GET_CUSTOMERS =
                $@"
            select
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
            where customer.aggr$state_id <> 4 and
                  is_payment_locked <> 1";

            public static readonly string GET_CUSTOMER_COUNTER_VALUES =
                $@"
            select first 1
                {COUNTER_NAME},
                {CUSTOMER_COUNTERS_END_DAY_VALUE},
                {CUSTOMER_COUNTERS_END_NIGHT_VALUE},
                {CUSTOMER_COUNTERS_IS_TWO_TARIFF}
            from R090$GET_COUNTER_VALUES({PARAM_CUSTOMER_ID}, {PARAM_START_DATE}, {PARAM_END_DATE})";

            public static readonly string GET_REASONS =
                $"select {REASON_ID}, {REASON_NAME}, {REASON_CANPAY} from reason";

            public static readonly string GET_PAYMENT_KIND =
                $@"
			select first 1 id {PAYMENT_KIND_ID}, name {PAYMENT_KIND_NAME}, paymenttypeid {PAYMENT_KIND_TYPE_ID}
			from paymentkind
			where paymentkind.id = {PARAM_PAYMENT_TYPE_ID}";

            public static readonly string GET_PAYMENT_KINDS =
                $@"
			select id {PAYMENT_KIND_ID}, name {PAYMENT_KIND_NAME}, paymenttypeid {PAYMENT_KIND_TYPE_ID}
			from paymentkind";

            public static readonly string INSERT_PAYMENT_KIND =
                $@"
			insert into paymentkind
			values({PARAM_PAYMENT_KIND_ID}, {PARAM_PAYMENT_KIND_NAME}, {PARAM_PAYMENT_TYPE_ID})
			returning id";

            public static readonly string GET_PAYJOURNAL =
                $@"
			select first 1 id {PAY_JOURNAL_ID}, name {PAY_JOURNAL_NAME}
			from payjournal
			where (payjournal.createdate = {PARAM_CREATE_DATE}) and
				  (payjournal.paymentkind_id = {PARAM_PAYMENT_KIND_ID})";

            public static readonly string UPDATE_PAYJOURNAL =
                $@"
			update payjournal
			set payjournal.requiresum = payjournal.requiresum + {PARAM_PAYMENT_COST},
                payjournal.requirecount = payjournal.requirecount + 1
			where (payjournal.id = {PARAM_PAY_JOURNAL_ID})";

            public static readonly string INSERT_PAY_JOUNAL =
                $@"
			insert into payjournal values(
				null,
				{PARAM_PAY_JOURNAL_NAME},
				{PARAM_CREATE_DATE},
				{PARAM_CREATE_DATE},
				{PARAM_PAYMENT_KIND_ID},
				{PARAM_PAYMENT_COST},
				1,
				null,
				1)
			returning id";

            public static readonly string INSERT_PAY =
                $@"
            insert into pay (ID, CUSTOMER_ID, PAYJOURNAL_ID, REASON_ID, METERS_ID, TOTAL, PENALTYTOTAL, DESCRIPTION)
                values(
                    null,
                    {PARAM_CUSTOMER_ID},
                    {PARAM_PAY_JOURNAL_ID},
                    {PARAM_REASON_ID},
                    {PARAM_METERS_ID},
                    {PARAM_PAYMENT_COST},
                    {PARAM_PENALTY_TOTAL},
                    {PARAM_DESCRIPTION})
			returning id";

            public static readonly string SELECT_CUSTOMER_COUNTER =
                $@"
			select customercounter.id {CUSTOMER_COUNTER_ID}
            from customercounter
            where customercounter.customer_id = {PARAM_CUSTOMER_ID} and
				  customercounter.unmountdate is null";

            public static readonly string INSERT_COUNTERVALUES =
                $@"
            insert into countervalues (ID, CUSTOMERID, CUSTOMERCOUNTERID, PAY_ID, RECEIVEDDATE, DAYVALUE, NIGHTVALUE, ISUSED, VOLUME_TYPE_ID)
                values (
                    null,
				    {PARAM_CUSTOMER_ID},
				    {PARAM_CUSTOMER_COUNTER_ID},
				    null,
				    {PARAM_CREATE_DATE},
                    {PARAM_VALUE1},
				    {PARAM_VALUE2},
				    0,
				    1)
			    returning id";

            public static readonly string UPDATE_COUNTERVALUES_PAY_ID =
                $@"
            update countervalues
			    set countervalues.PAY_ID = {PARAM_PAY_ID}
            where (countervalues.ID = {PARAM_COUNTER_VALUES_ID})";

            public static readonly string INSERT_METERS =
                $@"
            insert into meters values(
                null,
                {PARAM_CUSTOMER_ID},
                {PARAM_CUSTOMER_COUNTER_ID},
                {PARAM_VALUE1},
                0,
                {PARAM_VALUE2},
                0,
                1,
                {PARAM_COUNTER_VALUES_ID})
			returning id";

            public static readonly string SELECT_DEBT =
                $@"
            select first 1 {END_BALANCE}, {PENALTY_BALANCE}
            from r090$print({PARAM_CUSTOMER_ID}, {PARAM_DAY_ENCODING})";

            public static readonly string GET_WAREHOUSES =
                $@"
            select {WAREHOUSE_ID}, {WAREHOUSE_CODE}, {WAREHOUSE_NAME}, {WAREHOUSE_QUANTITY}, {WAREHOUSE_UNITNAME}, {WAREHOUSE_BARCODE}
            from warehouse";

            public static readonly string GET_WAREHOUSE_CATEGORIES =
                $@"
            select {WAREHOUSE_CATEGORY_ID}, {WAREHOUSE_CATEGORY_CODE}, {WAREHOUSE_CATEGORY_NAME}, {WAREHOUSE_CATEGORY_IS_DEFAULT}, {WAREHOUSE_CATEGORY_IS_WHOLESALE}
            from warehouse_category";

            public static readonly string GET_WAREHOUSE_PRICES =
                $@"
            select {WAREHOUSE_PRICE_ID}, {WAREHOUSE_PRICE_WAREHOUSE_ID}, {WAREHOUSE_PRICE_WAREHOUSE_CATEGORY_ID}, {WAREHOUSE_PRICE_ENTRY_DATE}, {WAREHOUSE_PRICE_PRICE_VALUE}
            from warehouse_price";
        }
    }
}
