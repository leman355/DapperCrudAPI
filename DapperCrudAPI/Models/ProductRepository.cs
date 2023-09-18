using Dapper;
using Npgsql;

namespace DapperCrudAPI.Models
{
    public class ProductRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        private NpgsqlConnection OpenConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public void Add(Product product)
        {
            using var connection = OpenConnection();

            using var transaction = connection.BeginTransaction();
            try
            {
                string insert = "INSERT INTO public.\"Product\"(\"Name\", \"Quantity\", \"Price\", \"IsDeleted\") " +
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
            using var connection = OpenConnection();
            return connection.Query<Product>("SELECT * FROM public.\"Product\"");
        }

        public Product GetById(int id)
        {
            using var connection = OpenConnection();
            string s = "SELECT * FROM public.\"Product\" WHERE \"Id\" = @Id";
            return connection.QueryFirstOrDefault<Product>(s, new { Id = id });
        }

        public void Delete(int id)
        {
            using var connection = OpenConnection();

            using var transaction = connection.BeginTransaction();
            try
            {
                string s = "DELETE FROM public.\"Product\" WHERE \"Id\" = @Id";
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
            using var connection = OpenConnection();

            using var transaction = connection.BeginTransaction();
            try
            {
                string s = "UPDATE public.\"Product\" SET \"Name\" = @Name, \"Quantity\" = @Quantity, \"Price\" = @Price, \"IsDeleted\" = @IsDeleted WHERE \"Id\" = @Id";
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
