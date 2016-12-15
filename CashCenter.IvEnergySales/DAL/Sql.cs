namespace CashCenter.IvEnergySales.DAL
{
    public static class Sql
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

	    public const string CUSTOMER_COUNTER_ID = "CUSTOMER_COUNTER_ID";

	    public const string PAY_JOURNAL_ID = "PAY_JOURNAL_ID";
	    public const string PAY_JOURNAL_NAME = "PAY_JOURNAL_NAME";

		public const string PAYMENT_KIND_ID = "PAYMENT_KIND_ID";
		public const string PAYMENT_KIND_NAME = "PAYMENT_KIND_NAME";

		public const string PARAM_CUSTOMER_ID = "@CUSTOMER_ID";
        public const string PARAM_START_DATE = "@START_DATE";
        public const string PARAM_END_DATE = "@END_DATE";
        public const string PARAM_PAYMENT_KIND_ID = "@PAYMENT_KIND_ID";
        public const string PARAM_PAYMENT_KIND_NAME = "@PAYMENT_KIND_NAME";
        public const string PARAM_CREATE_DATE = "@CREATE_DATE";
        public const string PARAM_PAYMENT_COST = "@PAYMENT_COST";
        public const string PARAM_PAY_JOURNAL_NAME = "@PAY_JOURNAL_NAME";
        public const string PARAM_REASON_ID = "@REASON_ID";
        public const string PARAM_DESCRIPTION = "@DESCRIPTION";
        public const string PARAM_VALUE1 = "@VALUE1";
        public const string PARAM_VALUE2 = "@VALUE2";
	    public const string PARAM_PAY_JOURNAL_ID = "@PAY_JOURNAL_ID";
	    public const string PARAM_CUSTOMER_COUNTER_ID = "@CUSTOMER_COUNTER_ID";
	    public const string PARAM_COUNTER_VALUES_ID = "@COUNTER_VALUES_ID";
	    public const string PARAM_PAY_ID = "@PAY_ID";

		public static readonly string GET_CUSTOMER =
            $@"select * from
                   (select first 1
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
                   where customer.id = {PARAM_CUSTOMER_ID} and customer.aggr$state_id <> 4) finded_customer,
                   (select first 1
                           {CUSTOMER_COUNTERS_END_DAY_VALUE},
                           {CUSTOMER_COUNTERS_END_NIGHT_VALUE},
                           {CUSTOMER_COUNTERS_IS_TWO_TARIFF}
                    from R090$GET_COUNTER_VALUES({PARAM_CUSTOMER_ID}, {PARAM_START_DATE}, {PARAM_END_DATE})) counter_values";

        public static readonly string GET_REASONS =
            $"select {REASON_ID}, {REASON_NAME}, {REASON_CANPAY} from reason";

	    public static readonly string GET_PAYMENT_KIND =
		    $@"
			select first 1 id {PAYMENT_KIND_ID}, name {PAYMENT_KIND_NAME}
			from paymentkind
			where paymentkind.name = {PARAM_PAYMENT_KIND_NAME}";

	    public static readonly string INSERT_PAYMENT_KIND =
		    $@"
			insert into paymentkind
			values(null, {PARAM_PAYMENT_KIND_NAME}, 1)
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
			set payjournal.requiresum += {PARAM_PAYMENT_COST},
                payjournal.requirecount += 1
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
				NULL,
				1)
			returning id";

        public static readonly string INSERT_PAY =
            $@"
            insert into pay (ID, CUSTOMER_ID, PAYJOURNAL_ID, REASON_ID, METERS_ID, TOTAL, PENALTYTOTAL, DESCRIPTION)
                values (
                    null,
                    {PARAM_CUSTOMER_ID},
                    {PARAM_PAY_JOURNAL_ID},
                    {PARAM_REASON_ID},
                    null,
                    {PARAM_PAYMENT_COST},
                    0,
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
            insert into countervalues values (
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
    }
}
