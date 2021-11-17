
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Stack.Core;
using Stack.Core.Managers;
using Stack.DTOs;
using Stack.DTOs.Enums;
using Stack.Entities.Models;
using Stack.ServiceLayer;
using Stack.DTOs.Models;
using Stack.Repository.Common;
using AutoMapper;
using Stack.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Stack.Entities;
using Stack.DTOs.Requests.Users;

namespace Stack.ServiceLayer
{
    public class UsersService
    {

        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public UsersService(UnitOfWork unitOfWork, IConfiguration config, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<bool>> InitializeSystemRoles()
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {

                var adminRoleExistsResult = await unitOfWork.RoleManager.RoleExistsAsync(UserRoles.Admin.ToString());
                var superAdminRoleExistsResult = await unitOfWork.RoleManager.RoleExistsAsync(UserRoles.SuperAdmin.ToString());

                if(adminRoleExistsResult == false)
                {
                    var roleToCreate = new IdentityRole();
                    roleToCreate.Name = UserRoles.Admin.ToString();

                    var createRoleRes = await unitOfWork.RoleManager.CreateAsync(roleToCreate);

                    foreach (var error in createRoleRes.Errors)
                    {
                        result.Errors.Add(error.Description);
                    }

                }

                if (superAdminRoleExistsResult == false)
                {
                    var roleToCreate = new IdentityRole();
                    roleToCreate.Name = UserRoles.SuperAdmin.ToString();

                    var createRoleRes = await unitOfWork.RoleManager.CreateAsync(roleToCreate);

                    foreach (var error in createRoleRes.Errors)
                    {
                        result.Errors.Add(error.Description);
                    }

                }

                if(result.Errors.Count == 0 )
                {

                    await unitOfWork.SaveChangesAsync();
                    result.Data = true;
                    result.Succeeded = true;
                    return result;

                }
                else
                {
                    result.Data = false;
                    result.Succeeded = false;
                    return result;
                }
               
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }

        }

        public async Task<ApiResponse<JwtAccessToken>> CreateSuperAdministratorAccount(UserRegistrationModel model)
        {

            ApiResponse<JwtAccessToken> result = new ApiResponse<JwtAccessToken>();

            try
            {

                var userExistsResult = await unitOfWork.ApplicationUserManager.FindByEmailAsync(model.Email);

                if (userExistsResult != null)
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed To Create Adminstrator, An account is already registered to this email address !");
                    result.ErrorType = ErrorType.LogicalError;
                    return result;
                }


                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var createUserResult = await unitOfWork.ApplicationUserManager.CreateAsync(user, model.Password);

                await unitOfWork.SaveChangesAsync();

                if (createUserResult.Succeeded)
                {
                    var roleresult = await unitOfWork.ApplicationUserManager.AddToRoleAsync(user, UserRoles.SuperAdmin.ToString());

                    var addToRoleResult = await unitOfWork.SaveChangesAsync();

                    if (roleresult.Succeeded == true)
                    {

                        // Creating JWT Bearer Token  
                        ClaimsIdentity claims = new ClaimsIdentity(new[]
                        {
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(ClaimTypes.NameIdentifier, user.Id)
                        });

                        IList<string> userRoles = await unitOfWork.ApplicationUserManager.GetRolesAsync(user);

                        if (userRoles != null && userRoles.Count() > 0)
                        {
                            foreach (string role in userRoles)
                            {
                                claims.AddClaim(new Claim(ClaimTypes.Role, role));
                            }
                        }

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Token:Key").Value));

                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                        var tokenExpiryDate = await HelperFunctions.GetEgyptsCurrentLocalTime();
                        tokenExpiryDate.AddDays(0.25);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(claims),
                            Expires = tokenExpiryDate, // Set Token Validity Period . 
                            SigningCredentials = creds
                        };

                        var tokenHandler = new JwtSecurityTokenHandler();

                        var token = tokenHandler.CreateToken(tokenDescriptor);

                        result.Data = new JwtAccessToken();
                        result.Data.Token = tokenHandler.WriteToken(token);
                        result.Data.Expiration = token.ValidTo;
                        result.Succeeded = true;
                        return result;

                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed To Create Adminstrator");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;

                    foreach (var error in createUserResult.Errors)
                    {
                        result.Errors.Add(error.Description);
                    }

                    result.ErrorType = ErrorType.LogicalError;
                    return result;

                }
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }

        }

        public async Task<ApiResponse<JwtAccessToken>> LoginAsync(LoginModel model)
        {
            ApiResponse<JwtAccessToken> result = new ApiResponse<JwtAccessToken>();
            try
            {

                //Find user by email . 
                var user = await unitOfWork.ApplicationUserManager.FindByEmailAsync(model.Email);

                if (user != null)
                {

                    //Check user password . 
                    bool res = await unitOfWork.ApplicationUserManager.CheckPasswordAsync(user, model.Password);

                    if (res)
                    {

                        // Creating JWT Bearer Token . 
                        ClaimsIdentity claims = new ClaimsIdentity(new[]
                        {
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim("FirstName", user.FirstName),
                                new Claim("LastName", user.LastName),
                                new Claim("Email", user.Email),
                                new Claim(ClaimTypes.NameIdentifier, user.Id)
                        });

                        IList<string> userRoles = await unitOfWork.ApplicationUserManager.GetRolesAsync(user);

                        if (userRoles != null && userRoles.Count() > 0)
                        {
                            foreach (string role in userRoles)
                            {
                                claims.AddClaim(new Claim(ClaimTypes.Role, role));
                            }
                        }

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Token:Key").Value));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                        var egyptsDateResult = await HelperFunctions.GetEgyptsCurrentLocalTime();


                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(claims),
                            NotBefore = egyptsDateResult,
                            Expires = egyptsDateResult.AddHours(8), // Set Token Validity Period. 
                            SigningCredentials = creds,
                            IssuedAt = egyptsDateResult
                        };

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.CreateToken(tokenDescriptor);

                        result.Data = new JwtAccessToken();
                        result.Data.Token = tokenHandler.WriteToken(token);
                        result.Data.Expiration = token.ValidTo;

                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Invalid login attempt.");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Invalid login attempt.");
                    result.ErrorType = ErrorType.LogicalError;
                    return result;
                }

            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }

        }

        public async Task<ApiResponse<ApplicationUserDTO>> EditSuperAdministratorAccountDetails(EditSuperAdminAccountModel model)
        {

            ApiResponse<ApplicationUserDTO> result = new ApiResponse<ApplicationUserDTO>();

            try
            {
                var usersResult = await unitOfWork.ApplicationUserManager.FindByEmailAsync(model.Email);

                if (usersResult != null) // if an account with a similar email address was found . 
                {
                   
                    if(usersResult.Id == model.Id)
                    {

                        usersResult.Email = model.Email;
                        usersResult.UserName = model.Email;
                        usersResult.FirstName = model.FirstName;
                        usersResult.LastName = model.LastName;


                        var updateUserResult = await unitOfWork.ApplicationUserManager.UpdateAsync(usersResult);

                        if(updateUserResult.Succeeded == true)
                        {

                            await unitOfWork.SaveChangesAsync();
                            result.Data = mapper.Map<ApplicationUserDTO>(usersResult);
                            result.Succeeded = true;
                            return result;

                        }
                        else
                        {

                            result.Succeeded = false;
                            result.Errors.Add("Failed to edit administrator account, Please try again !");
                            result.ErrorType = ErrorType.SystemError;
                            return result;

                        }


                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to edit administrator account, an account with a similar email address already exists !");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }

                }
                else // if an account with a similar email address was not found . 
                {

                    var userToUpdate = await unitOfWork.ApplicationUserManager.FindByIdAsync(model.Id);

                    if (userToUpdate != null)
                    {

                        userToUpdate.Email = model.Email;
                        userToUpdate.FirstName = model.FirstName;
                        userToUpdate.LastName = model.LastName;

                        var updateUserResult = await unitOfWork.ApplicationUserManager.UpdateAsync(userToUpdate);

                        if (updateUserResult.Succeeded == true)
                        {

                            await unitOfWork.SaveChangesAsync();
                            result.Data = mapper.Map<ApplicationUserDTO>(userToUpdate);
                            result.Succeeded = true;
                            return result;

                        }
                        else
                        {

                            result.Succeeded = false;
                            result.Errors.Add("Failed to edit administrator account, Please try again !");
                            result.ErrorType = ErrorType.SystemError;
                            return result;

                        }

                    }
                    else
                    {

                        result.Succeeded = false;
                        result.Errors.Add("Failed to edit administrator account, Please try again !");
                        result.ErrorType = ErrorType.SystemError;
                        return result;

                    }

                }

            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }

        }

        public async Task<ApiResponse<bool>> EditSuperAdministratorPassword(EditSuperAdminPasswordModel model)
        {

            ApiResponse<bool> result = new ApiResponse<bool>();

            try
            {
                var usersResult = await unitOfWork.ApplicationUserManager.FindByEmailAsync(model.Email);

                if (usersResult != null) // if an account with a similar email address was found .
                {

                    var changePasswordResult = await unitOfWork.ApplicationUserManager.ChangePasswordAsync(usersResult, model.OldPassword, model.NewPassword);
 
                    if (changePasswordResult.Succeeded == true)
                    {

                        await unitOfWork.SaveChangesAsync();

                        result.Data = true;
                        result.Succeeded = true;
                        return result;

                    }
                    else
                    {

                        result.Succeeded = false;
                        result.Errors.Add("Failed to update administrator's password, Please try again !");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;

                    }

                }
                else // if an account with a similar email address was not found . 
                {

                    result.Succeeded = false;
                    result.Errors.Add("Failed to update administrator's password, Please try again !");
                    result.ErrorType = ErrorType.SystemError;
                    return result;

                }

            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }

        }


    }

}


