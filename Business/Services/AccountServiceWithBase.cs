using AppCore.Business.Models.JsonWebToken;
using AppCore.Business.Models.Results;
using AppCore.Business.Utils.JsonWebToken.Bases;
using Business.Enums;
using Business.Models;
using System;
using System.Globalization;

namespace Business.Services
{
    public interface IAccountService
    {
        Result Register(UserRegisterModel model);
        Result<UserModel> Login(UserLoginModel model);
        Result<Jwt> CreateJwt(UserModel model);
    }

    public class AccountService : IAccountService
    {
        private readonly IUserService _userService;
        private readonly JwtUtilBase _jwtUtil;

        public AccountService(IUserService userService, JwtUtilBase jwtUtil)
        {
            _userService = userService;
            _jwtUtil = jwtUtil;
        }

        public Result<UserModel> Login(UserLoginModel model)
        {
            try
            {
                var result = _userService.GetUser(u => u.UserName == model.UserName && u.Password == model.Password && u.Active);
                if (result.Status == ResultStatus.Success)
                    result.Message = "User: " + result.Data.UserName + " logged in on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"));
                return result;
            }
            catch (Exception exc)
            {
                return new ExceptionResult<UserModel>(exc);
            }
        }

        public Result Register(UserRegisterModel model)
        {
            try
            {
                var user = new UserModel()
                {
                    Active = true,
                    UserName = model.UserName.Trim(),
                    Password = model.Password.Trim(),
                    RoleId = (int)Roles.User,
                    UserDetail = new UserDetailModel()
                    {
                        Address = model.UserDetail.Address.Trim(),
                        CityId = model.UserDetail.CityId,
                        CountryId = model.UserDetail.CountryId,
                        EMail = model.UserDetail.EMail.Trim()
                    }
                };
                return _userService.Add(user);
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result<Jwt> CreateJwt(UserModel model)
        {
            return _jwtUtil.CreateJwt(model.UserName, model.Role.Name, model.Id.ToString());
        }
    }
}
