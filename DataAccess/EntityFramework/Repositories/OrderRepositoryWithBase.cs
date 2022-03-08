using AppCore.DataAccess.Bases.EntityFramework;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EntityFramework.Repositories
{
    public abstract class OrderRepositoryBase : RepositoryBase<Order>
    {
        protected OrderRepositoryBase(DbContext db) : base(db)
        {

        }
    }

    public class OrderRepository : OrderRepositoryBase
    {
        public OrderRepository(DbContext db) : base(db)
        {

        }
    }
}
