using Dapper;
using Npgsql;

namespace DapperCrudAPI.Models
{
    public class ProductRepository
    {
        private readonly IConfiguration _configuration;

        public ProductRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Add(Product product)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                string insert = "INSERT INTO public.\"Products\"(\"Name\", \"Quantity\", \"Price\", \"IsDeleted\") " +
                                "VALUES(@Name, @Quantity, @Price, @IsDeleted) RETURNING \"Id\"";
                int insertedId = connection.ExecuteScalar<int>(insert, product, transaction: transaction);
                product.Id = insertedId;
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public IEnumerable<Product> GetAll()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            return connection.Query<Product>("SELECT * FROM Products");
        }

        public Product GetById(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            string s = "SELECT * FROM Products WHERE Id = @Id";
            return connection.QueryFirstOrDefault<Product>(s, new { Id = id });
        }

        public void Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                string s = "DELETE FROM Products WHERE Id = @Id";
                connection.Execute(s, new { Id = id }, transaction: transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public void Update(Product product)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                string s = "UPDATE Products SET Name = @Name, Quantity = @Quantity, Price = @Price, IsDeleted = @IsDeleted WHERE Id = @Id";
                connection.Execute(s, product, transaction: transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}