using AngularBackEnd.Models.CustomerManagement;

namespace AngularBackEnd.Services.ProductManagement
{
    public class ProductService : IProductService
    {
        private readonly List<Product> _products;

        public ProductService()
        {
            _products = new List<Product>
            {
                new Product { Id = 1, Name = "Milk", Price = 20000 },
                new Product { Id = 2, Name = "Bread", Price = 15000 },
                new Product { Id = 3, Name = "Eggs", Price = 30000 },
                new Product { Id = 4, Name = "Cheese", Price = 50000 },
                new Product { Id = 5, Name = "Butter", Price = 40000 }
            };
        }

        public void Add(Product product)
        {
            product.Id = _products.Max(p => p.Id) + 1;
            _products.Add(product);
        }

        public void Delete(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
        }

        public List<Product> GetAll()
        {
            return _products;
        }

        public Product GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public void Update(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Price = product.Price;
            }
        }
    }
}
