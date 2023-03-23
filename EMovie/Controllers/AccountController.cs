using EMovie.Data;
using EMovie.Data.Static;
using EMovie.Data.ViewData;
using EMovie.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EMovie.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicaitonUser> _userManager;
        private readonly SignInManager<ApplicaitonUser> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<ApplicaitonUser> userManager, SignInManager<ApplicaitonUser> signInManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
            if (user!=null)
            {
                var checkPassword = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (checkPassword)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Movies");
                    }
                }
            }


            TempData["Error"] = "Wrong credentials. Please try again!";
            return View(loginVM);


        }

        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) { return View(registerVM); }

            var userExists = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
            if(userExists != null)
            {
                TempData["Error"] = "Email is already in use";
                return View(registerVM);
            }
            var newUser = new ApplicaitonUser()
            {
                FullName = registerVM.FullName,
                Email = registerVM.EmailAddress,
                UserName = registerVM.EmailAddress
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
            }
            else
            {
                TempData["Error"] = newUserResponse.Errors.FirstOrDefault().Description;
                return View(registerVM);
            }
            return View("RegisterComplete");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Movies");
        }
    }
}
