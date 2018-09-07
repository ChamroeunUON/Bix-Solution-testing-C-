namespace BizSolution
{
    public class Product
    {
        public int Id { get; set; }
        public string ProcustName { get; set; }
        public float Price { get; set; }

        public float GetPrice()
        {
            return this.Price;
        }
        public string GetProductName()
        {
            return this.ProcustName;
        }
    }
}