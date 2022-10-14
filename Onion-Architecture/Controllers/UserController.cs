using Microsoft.AspNetCore.Mvc;
using OA.Data;
using OA.Repository;
using OA.Service;
using Onion_Architecture.Models;
using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace Onion_Architecture.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService userService;

        private readonly IUserInfoService userInfoService;

        public UserController(IUserService userService, IUserInfoService userInfoService)
        {
            this.userService = userService;
            this.userInfoService = userInfoService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<UserViewModel> model = new List<UserViewModel>();

            userService.GetAllUser().ToList().ForEach(u =>
            {
                UserInfo userInfo = userInfoService.GetUserInfo(u.Id);
                UserViewModel user = new UserViewModel
                {
                    Id = u.Id,
                    Name = $"{userInfo.FirstName} {userInfo.LastName}",
                    Email = u.Email,
                    Address = userInfo.Address
                };
                model.Add(user);
            });
            return View(model);
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            UserViewModel model = new UserViewModel();
            return View("AddUser", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(UserViewModel model)
        {
                User user = new User
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    Email = model.Email,
                    AddedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserInfo = new UserInfo
                    {
                        Address = model.Address,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        AddedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString()
                    }
                };
                userService.InsertUser(user);
                if (user.Id > 0)
                {
                    return RedirectToAction("Index");
                }
            return View(model);
        }

        [HttpGet]
        public IActionResult EditUser(int? id)
        {
            UserViewModel usermodel = new UserViewModel();
            if(id.HasValue && id.Value > 0)
            {
                User user = userService.GetUser(id.Value);
                UserInfo userInfo = userInfoService.GetUserInfo(id.Value);
                usermodel.FirstName = userInfo.FirstName;
                usermodel.LastName = userInfo.LastName;
                usermodel.Address = userInfo.Address;
                usermodel.Email = user.Email;
            }
            return View("EditUser", usermodel);
        }

        [HttpPost]
        public IActionResult EditUser(UserViewModel model)
        {
            User user = userService.GetUser(model.Id);
            UserInfo userInfo = userInfoService.GetUserInfo(model.Id);
            user.Email = model.Email;
            user.ModifiedDate = DateTime.UtcNow;
            user.IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            userInfo.FirstName = model.FirstName;
            userInfo.LastName = model.LastName;
            userInfo.Address = model.Address;
            userInfo.ModifiedDate = DateTime.UtcNow;
            userInfo.IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            user.UserInfo = userInfo;
            userService.UpdateUser(user);
            if(user.Id >0)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteUser(UserViewModel model,long ?id)
        {
            UserInfo userInfo = userInfoService.GetUserInfo(id);
            User user = userService.GetUser(id);
            return View("DeleteUser", model);
        }

        [HttpPost]
        public IActionResult DeleteUser(long? id)
        {
            userService.DeleteUser(id);
            return RedirectToAction("Index");
        }
    }
}
