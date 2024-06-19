using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Authen;
using Data.ViewModel.Helper;
using Data.ViewModel.System;
using Data.ViewModel.User;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Service.Helper.Email;
using Service.Helper.Media;
using Service.Interface;
using System.Collections;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailTemplateReader _emailTemplateReader;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IEmailHelper _emailHelper;
        private readonly IMediaHelper _mediaHelper;

        public UserService(IUnitOfWork unitOfWork,
            IMediaHelper mediaHelper,
            IEmailTemplateReader emailTemplateReader,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IEmailHelper emailHelper)
        {
            _mediaHelper = mediaHelper;
            _roleManager = roleManager;
            _emailHelper = emailHelper;
            _emailTemplateReader = emailTemplateReader;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<User>> CheckLogin(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Username);
            if (user == null)
            {
                return new ApiResult<User>() { Success = false, message = "Username is incorrect!" };
            }

            var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!result)
            {
                return new() { Success = false, message = "Password is incorrect!" };
            }

            if (!user.EmailConfirmed)
            {
                return new()
                {
                    Success = false,
                    message = "Your account is not activated!"
                };
            }


            return new ApiResult<User>()
            {
                Success = true,
                Value = user,
            };
        }

        public async Task<ApiResult<string>> BanUser(string username, TimeAddInput time)
        {
            var user = await UserExits(username);
            DateTime now = DateTime.Now;
            if (time.Minutes > 0)
            {
                now = now.AddMinutes((double)time.Minutes);
            }
            if (time.Hours > 0)
            {
                now = now.AddMinutes((double)time.Hours);
            }
            if (time.Days > 0)
            {
                now = now.AddMinutes((double)time.Days);
            }
            if (time.Months > 0)
            {
                now = now.AddMinutes((double)time.Months);
            }
            if (time.Years > 0)
            {
                now = now.AddMinutes((double)time.Years);
            }

            await _unitOfWork.RepositoryUserBan.Insert(new UserBan()
            {
                endDate = now,
                UserId = user.Id
            });

            try
            {

                await _unitOfWork.CommitAsync();
                return new ApiResult<string>
                {
                    Success = true,
                    message = $"{user.UserName} have been ban to {now.ToString("HH:mm - dd/M/yyyy")}"
                };

            }
            catch (Exception ex)
            {
                return new ApiResult<string>()
                {
                    message = ex.Message,
                    Success = false,
                };
            }

        }
    
        public async Task<IEnumerable<User>> GetAll()
        {
            return await _unitOfWork.RepositoryUser.GetAll();
        }
        public async Task<IEnumerable<User>> Search(string key, int pageIndex, int pageSize)
        {
            return await _unitOfWork.RepositoryUser.GetPageSize(x =>
            x.UserName.Contains(key) ||
            x.Firstname.Contains(key) ||
            x.Lastname.Contains(key) ||
            x.Email.Contains(key) ||
            x.Address.Contains(key),
            pageIndex: pageIndex,
            pageSize: pageSize);
        }
        public async Task<IEnumerable<User>> GetPageSize(int pageIndex, int pageSize)
        {
            return await _unitOfWork.RepositoryUser.GetPageSize(pageIndex: pageIndex, pageSize: pageSize);
        }

        public async Task<User> FindByUsername(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }
        public async Task<User> FindByEmail(string Email)
        {
            return await _userManager.FindByEmailAsync(Email);
        }

        public async Task RemoveRole(Guid id, string[] roles)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            await _userManager.RemoveFromRolesAsync(user, roles);
        }
        public async Task<bool> CheckUserBan(Guid userId)
        {
            var luserban = await _unitOfWork.RepositoryUserBan.GetAll(x => x.UserId == userId);
            var userban = luserban.LastOrDefault();

            if (userban == null)
            {
                return false;
            }
            if (userban.endDate > DateTime.Now)
            {
                return true;
            }
            return false;
        }
        public async Task<User> FindById(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());

        }

        public async Task<(ApiResult<User>, string)> Register(RegisterRequest registerRequest)

        {
            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                return new(new ApiResult<User>
                { Success = false, message = "Comfirm password not match" }, "");
            }

            var user = await _userManager.FindByNameAsync(registerRequest.UserName);

            if (user != null)
            {
                return new(new ApiResult<User>
                { Success = false, message = "Username already exits" }, "");
            }

            if (await _userManager.FindByEmailAsync(registerRequest.Email) != null)
            {
                return new(new ApiResult<User>
                { Success = false, message = "Email already exits" }, "");
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            DateOnly minDate = today.AddYears(-60);
            DateOnly maxDate = today.AddYears(-18);
            if (!(registerRequest.Birthday <= maxDate) || !(registerRequest.Birthday >= minDate))
            {
                return new(new()
                {
                    Success = false,
                    message = $"{registerRequest.Birthday} is too young or too old"
                }, "");
            }

            var newUser = new User()
            {
                Birthday = registerRequest.Birthday,
                Address = registerRequest.Address,
                Firstname = registerRequest.FirstName,
                Lastname = registerRequest.LastName,
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber,

            };
            var result = await _userManager.CreateAsync(newUser, registerRequest.Password);


            if (!result.Succeeded)
            {
                return new(new ApiResult<User>
                { Success = false, message = "Something fail when Created" }, "");

            }


            string tokenComfirmEmail = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenComfirmEmail));

            await _userManager.AddToRoleAsync(newUser, "customer");

            await _unitOfWork.RepositoryUserStatusLog.Insert(new UserStatusLog
            {
                LogAt = DateTime.Now,
                StatusId = 2,
                UserId = newUser.Id,
                TextLog = newUser.UserName + " Create at " + DateTime.Now.ToString(),
            });
            await _unitOfWork.CommitAsync();
            return new(new ApiResult<User>
            { Value = newUser, Success = true, message = "Created" }, encodedToken);
        }

        public async Task<ApiResult<User>> UpdateUser(UpdateUserRequest updateUserRequest)
        {
            var user = await _userManager.FindByNameAsync(updateUserRequest.Username.ToString());
            if (user == null)
            {
                return new()
                {
                    Success = false,
                    message = "User is not exist"
                };
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            DateOnly minDate = today.AddYears(-60);
            DateOnly maxDate = today.AddYears(-18);
            if (updateUserRequest.BirthDay.HasValue)
                if (!(updateUserRequest.BirthDay <= maxDate) || !(updateUserRequest.BirthDay >= minDate))
                {
                    return new()
                    {
                        Success = false,
                        message = $"{updateUserRequest.BirthDay} is too young or too old"
                    };
                }
                else
                {
                    user.Birthday = updateUserRequest.BirthDay;
                }
            if (updateUserRequest.Avatar != null)


                if (!FileValidationHelper.IsValidImage(updateUserRequest.Avatar))
                {
                    return new()
                    {
                        Success = false,
                        message = $"Avatar upload is not a image"
                    };
                }

                else
                {
                    var saveimage = await _mediaHelper.SaveMedia(updateUserRequest.Avatar, "User");
                    if (saveimage != null)
                    {
                        user.Avatar = saveimage.url;
                    }
                }


            if (!updateUserRequest.FirstName.IsNullOrEmpty())
            {
                user.Firstname = updateUserRequest.FirstName;
            }
            if (!updateUserRequest.LastName.IsNullOrEmpty())
            {
                user.Lastname = updateUserRequest.LastName;
            }
            if (!updateUserRequest.Address.IsNullOrEmpty())
            {
                user.Address = updateUserRequest.Address;
            }
            if (!updateUserRequest.Phonenumber.IsNullOrEmpty())
            {
                user.PhoneNumber = updateUserRequest.Phonenumber;
            }



            await _userManager.UpdateAsync(user);

            await _unitOfWork.RepositoryUserStatusLog.Insert(new UserStatusLog
            {
                LogAt = DateTime.Now,
                StatusId = 2,
                UserId = user.Id,
                TextLog = user.UserName + " Update at " + DateTime.Now.ToString(),
            });
            await _unitOfWork.CommitAsync();

            return new()
            {
                Success = true,
                message = $"{user.UserName} is have been updated"
            };
        }


        public async Task<bool> SendEmailComfirm(string url, User user)
        {

            try
            {
                string body = await _emailTemplateReader.GetTemplate("ComfirmEmail.html");
                body = string.Format(body, user.UserName, url);

                await _emailHelper.SendEmail(new EmailRequest
                {
                    To = user.Email,
                    Subject = "Confirm Email In Crafted Joy",
                    Content = body
                });

                return true;
            }
            catch { return false; }


        }

        public async Task<ApiResult<bool>> ComfirmEmail(string userId, string tokenConfirm)
        {


            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new()
                {
                    Success = false,
                    message = "Account is not exist!"
                };
            }

            if (user.EmailConfirmed)
            {
                return new()
                {
                    Success = false,
                    message = "Email has already been confirmed"
                };
            }
            string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(tokenConfirm));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                return new()
                {
                    Success = false,
                    message = "Comfirm email failed."
                };
            }

            return new()
            {
                Success = true,
                message = "your account has been actived",
            };
        }

        public async Task<ApiResult<bool>> ForgetPassword(string email, string host)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new()
                {
                    Success = false,
                    message = "Email is not exist!"
                };
            }

            if (host == null)
            {
                return new()
                {
                    Success = false,
                    message = "Ivalid http"
                };
            }

            string tokenConfirm = await _userManager.GeneratePasswordResetTokenAsync(user);

            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenConfirm));
            string resetPasswordUrl = $"{host}/ResetPassword?email={email}&token={encodedToken}";

            string body = $"Please reset your password by clicking here : <a href=\"{resetPasswordUrl}\">Reset Password </a>";

            await _emailHelper.SendEmail(new EmailRequest
            {
                To = user.Email,
                Subject = "Reset password",
                Content = body,
            });


            return new()
            {
                Success = true,
                message = "Check your email!"
            };
        }

        public async Task<ApiResult<bool>> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);

            if (user == null)
            {
                return new()
                {
                    Success = false,
                    message = "Email is not exist"
                };
            }

            if (string.IsNullOrEmpty(resetPasswordRequest.ResetCode))
            {
                return new()
                {
                    Success = false,
                    message = "Token is invalid"
                };
            }

            string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordRequest.ResetCode));

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordRequest.NewPassword);

            if (!result.Succeeded)
            {
                return new()
                {
                    Success = false,
                    message = "Reset password fail"
                };
            }
            return new()
            {
                Success = true,
                message = "Reset password successful"
            };
        }

        public async Task<ApiResult<bool>> UpdateRole(UpdateRoleRequest updateRoleRequest)
        {
            var user = await _userManager.FindByNameAsync(updateRoleRequest.Username);
            if (user == null)
            {
                return new()
                {
                    Success = false,
                    message = "User is not exist!"
                };
            }


            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleRemove = new List<string>();
            var roleAdd = new List<string>();


            foreach (var role in roles)
            {

                if (userRoles.Any(x => x == role.Name))
                {
                    if (!updateRoleRequest.roles.Any(x => x == role.Name))
                    {
                        roleRemove.Add(role.Name);
                    }

                }
                else
                {
                    if (updateRoleRequest.roles.Any(x => x == role.Name))
                    {
                        roleAdd.Add(role.Name);
                    }
                }
            }


            await _userManager.RemoveFromRolesAsync(user, roleRemove);
            await _userManager.AddToRolesAsync(user, roleAdd);

            userRoles = await _userManager.GetRolesAsync(user);


            return new()
            {
                Success = true,
                message = $"{user.UserName} add role : {string.Join(", ", userRoles.ToArray())} successful"
            };
        }

        public async Task<User> UserExits(string Username)
        {
            if (Username.IsNullOrEmpty())
            {
                throw new Exception("Username is not exits");
            }

            var user = await _userManager.FindByNameAsync(Username);
            if (user == null)
            {
                throw new Exception("Username is not exits");
            }

            return user;

        }

        public async Task<ApiResult<bool>> DeleteUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new ApiResult<bool> { Success = false, message = "User not found." };
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return new ApiResult<bool> { Success = false, message = "Failed to delete the user." };
            }

            // Logging the deletion
            await _unitOfWork.RepositoryUserStatusLog.Insert(new UserStatusLog
            {
                LogAt = DateTime.Now,
                StatusId = 5, // Assuming 5 indicates a deletion
                UserId = user.Id,
                TextLog = $"User {user.UserName} was deleted."
            });
            await _unitOfWork.CommitAsync();

            return new ApiResult<bool> { Success = true, message = "User deleted successfully." };
        }

        
    }
}