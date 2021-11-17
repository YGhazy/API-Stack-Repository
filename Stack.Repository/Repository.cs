using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Stack.Repository
{
    public class Repository<TEntity, TContext> : IRepository<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        public TContext context;
        public DbSet<TEntity> dbSet;

        public Repository(TContext _context)
        {
            context = _context;
            dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<IQueryable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            int pageNumber = 0,
            int itemsPerPage = 0,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (!string.IsNullOrEmpty(includeProperties) && !string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrEmpty(includeProperty))
                    {
                        query = query.Include(includeProperty);
                    }
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                if (pageNumber > 0 && itemsPerPage > 0)
                {
                    query = orderBy(query)
                        .Skip((pageNumber - 1) * itemsPerPage)
                        .Take(itemsPerPage);
                }
                else
                {
                    query = orderBy(query); ;
                }
            }
            else
            {
                if (pageNumber > 0 && itemsPerPage > 0)
                {
                    query = query
                        .Skip((pageNumber - 1) * itemsPerPage)
                        .Take(itemsPerPage);
                }
            }
            return await Task.Run(() =>
            {
                return query;
            });
        }

        public virtual async Task<TEntity> GetByIdAsync(params object[] id)
        {
            var item = await dbSet.FindAsync(id);
            //if (item != null && !item.IsDeleted)
            //{
            return item;
            //}
            //return null;
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            //entity.CreatedOn = DateTime.UtcNow;
            var dbChangeTracker = await dbSet.AddAsync(entity);
            return dbChangeTracker.State == EntityState.Added ? dbChangeTracker.Entity : null;
        }

        public virtual async Task CreateRangeAsync(params TEntity[] entities)
        {
            //foreach (var entity in entities)
            //{
            //    entity.CreatedOn = DateTime.UtcNow;
            //}
            await dbSet.AddRangeAsync(entities);
        }

        public virtual async Task<bool> UpdateAsync(TEntity entityToUpdate)
        {
            return await Task.Run(() =>
            {
                //entityToUpdate.ModifiedOn = DateTime.UtcNow;
                var dbChangeTracker = dbSet.Update(entityToUpdate);
                return dbChangeTracker.State == EntityState.Modified;
            });
        }

        public virtual async Task<bool> RemoveAsync(TEntity entity)
        {
            return await Task.Run(() =>
            {
                if (entity != null)
                {
                    //entity.IsDeleted = true;
                    //return await UpdateAsync(entityToDelete) != null ? true : false;
                    var dbChangeTracker = dbSet.Remove(entity);
                    return dbChangeTracker.State == EntityState.Deleted;
                }
                return false;
            });

        }

        public async Task<List<Dictionary<string, object>>> ExecuteStoredProcedure(string storedProcedureName, Dictionary<string, object> parameters = null)
        {
            return await Task.Run(() =>
            {
                
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                string connectionString = context.Database.GetDbConnection().ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(storedProcedureName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", param.Key), param.Value));
                    }
                }
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                Dictionary<string, object> dic;
                string colName;
                object colData;
                foreach (DataRow row in dt.Rows)
                {
                    dic = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        var colNameCamelCase = col.ColumnName.ToCharArray();
                        colNameCamelCase[0] = colNameCamelCase[0].ToString().ToLower().ToCharArray()[0];
                        colName = new string(colNameCamelCase);
                        colData = row[col];
                        dic.Add(colName, colData);
                    }
                    result.Add(dic);
                }
                return result;
            });

        }
    }
}
