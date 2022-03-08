using AppCore.DataAccess.Bases.EntityFramework;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EntityFramework.Repositories
{
    public abstract class ProductOrderRepositoryBase : RepositoryBase<ProductOrder>
    {
        protected ProductOrderRepositoryBase(DbContext db) : base(db)
        {

        }
    }

    public class ProductOrderRepository : ProductOrderRepositoryBase
    {
        public ProductOrderRepository(DbContext db) : base(db)
        {

        }
    }
}
