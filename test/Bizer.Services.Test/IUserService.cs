﻿namespace Bizer.Services.Test
{
    [InjectService]
    [ApiRoute("api/users")]
    public interface IUserService:ICrudService<int,User>
    {
    }

    public class UserService : BizerCrudServiceBase<int, User>, IUserService
    {
        public UserService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
