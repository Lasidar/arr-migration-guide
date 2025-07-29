﻿using System;
using System.Linq;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Authentication
{
    public interface IUserRepository : IBasicRepository<User>
    {
        User FindUser(string username);
        User FindUser(Guid identifier);
    }

    public class UserRepository : BasicRepository<User>, IUserRepository
    {
        public UserRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public User FindUser(string username)
        {
            return Query(x => x.Username == username).SingleOrDefault();
        }

        public User FindUser(Guid identifier)
        {
            return Query(x => x.Identifier == identifier).SingleOrDefault();
        }
    }
}
