﻿using System.Collections;
using Default_Project.Cores;
using Default_Project.Cores.Interfaces;
using Default_Project.Cores.Models;
using Default_Project.Repos.Data;

namespace Default_Project.Repos
{
    public class UnitWork : IUnitWork
    {
        private readonly StoreContext _dbContext;
        private readonly Hashtable _repos = new Hashtable();

        public UnitWork(StoreContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        public async Task<int> CompleteAsync() => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync() => await _dbContext.DisposeAsync();

        public IGenericRepo<TEntity> Repo<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity).Name;
            if (!_repos.ContainsKey(type))
            {
                var repo = new GenericRepo<TEntity>(_dbContext);
                _repos.Add(type, repo);
            }
            return (_repos[type] as IGenericRepo<TEntity>)!;
        }
    }
}
