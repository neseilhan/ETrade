using AppCore.DataAccess.Bases.EntityFramework;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EntityFramework.Repositories
{
    public abstract class UserDetailRepositoryBase : RepositoryBase<UserDetail>
    {
        protected UserDetailRepositoryBase(DbContext db) : base(db)
        {

        }
    }

    public class UserDetailRepository : UserDetailRepositoryBase
    {
        public UserDetailRepository(DbContext db) : base(db)
        {

        }
    }
}
