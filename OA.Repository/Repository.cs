using Microsoft.EntityFrameworkCore;
using OA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationContext context;

        private DbSet<T> entities;

        string errorString = string.Empty;

        private DbSet<User> users;

        private User user1 = new User();

        public Repository(ApplicationContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }
        
        public void Delete(T entity)
        {
            if(entity == null)
            {
                throw new ArgumentNullException("Entity");
            }
            else
            {
                entities.Remove(entity);
                context.SaveChanges();
            }
        }


        public bool GetUser(string username, string password)
        {
            //User user = GetUserByUsername(username);
            var currentuser = users.FirstOrDefault(s=>s.UserName.ToLower() ==  username.ToLower() && s.Password.ToLower() == password.ToLower());
            if(currentuser != null)
            {
                return true;
            }
            return false;
        }

        public T Get(long? id)
        {
            return entities.SingleOrDefault(s => s.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return entities.AsEnumerable();
        }

        public void Insert(T entity)
        {
            if (entity == null)
            { 
                throw new ArgumentNullException("Entity");
            }
            else
            {
                entities.Add(entity);
                context.SaveChanges();
            }
        }

        public void Remove(T entity)
        {
            if(entity == null)
            {
                throw new ArgumentNullException("Entity");
            }
            entities.Remove(entity);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Entity");
            }
            context.SaveChanges();
        }

        public T GetByPredicate(Func<T, bool> func)
        {
            return entities.FirstOrDefault(func);
        }

        //public User GetUserByUsername(string username)
        //{
        //    foreach(var us in entities)
        //    {
        //        if(us.Id == username)
        //        {
        //            user1 = us;
        //        }
        //    }
        //    if (user1 != null)
        //    {
        //        return user1;
        //    }
        //    return null;
        //}
    }
}
