namespace CashCenter.DbfRegistry.Entities
{
    public class Article
    {
        public string Code { get; private set; }

        public string Name { get; private set; }

        public string Barcode { get; private set; }

        public decimal Price { get; private set; }

        public Article(string code, string name, string barcode, decimal price)
        {
            Code = code;
            Name = name;
            Barcode = barcode;
            Price = price;
        }
    }
}
