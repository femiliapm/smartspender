using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PlannerTracker.DataAccess.AddOns;
using PlannerTracker.DataModel;
using PlannerTracker.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace PlannerTracker.DataAccess
{
    public class DAAuth
    {
        private readonly PlannerTrackerContext _db;
        private readonly Hashing hash;
        private readonly string jwtIssuer;
        private readonly string jwtKey;

        public DAAuth(PlannerTrackerContext db, IConfiguration config)
        {
            _db = db;
            hash = new();
            jwtIssuer = config["Jwt:Issuer"];
            jwtKey = config["Jwt:Key"];
        }

        public VMResponse<VMUser> Register(VMAuth vUser)
        {
            //VMResponse<VMUser> response = new VMResponse<VMUser>();
            VMResponse<VMUser> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    // check unique email user
                    User? user = _db.Users.Where(u => u.Email == vUser.Email && u.IsDelete == false).FirstOrDefault();

                    if (user != null)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Email is already registered!";
                        return response;
                    }

                    // Persiapkan entitas model
                    user = new User()
                    {
                        Email = vUser.Email,
                        FullName = vUser.FullName,
                        PasswordHash = hash.HashPassword(vUser.Password),
                        Username = vUser.Username ?? string.Empty
                    };

                    // save to db
                    _db.Add(user);
                    _db.SaveChanges();

                    //if (vUser.Role == null) vUser.Role = "User";
                    vUser.Role ??= "User"; // other implementation of code above


                    // find role user
                    Role? role = _db.Roles.Where(r => r.RoleName.ToLower() == vUser.Role.ToLower()).FirstOrDefault();

                    if (role == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Role {vUser.Role} is not found!";
                        return response;
                    }

                    UserRole userRole = new()
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    };

                    _db.Add(userRole);
                    _db.SaveChanges();

                    // commit transaction
                    dbTran.Commit();

                    // response assign value
                    response.Data = new VMUser(user);
                    response.StatusCode = HttpStatusCode.Created;
                    response.Message = $"User is successfully created!";
                }
                catch (Exception ex)
                {
                    // rollback transaction
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public VMResponse<VMAuth> Login(VMAuth vAuth)
        {
            VMResponse<VMAuth> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    // check apakah email terdaftar atau tidak
                    User? user = _db.Users.Where(u => u.Email == vAuth.Email && u.IsDelete == false).FirstOrDefault();

                    if (user == null)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Email is not registered!";
                        return response;
                    }

                    // check apakah passwordnya sama atau tidak
                    if (!hash.ValidatePassword(vAuth.Password, user.PasswordHash))
                    {
                        response.StatusCode = HttpStatusCode.Unauthorized;
                        response.Message = "Bad credentials!";
                        return response;
                    }

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                    var secToken = new JwtSecurityToken(jwtIssuer, jwtIssuer, null, expires: DateTime.Now.AddMinutes(120), signingCredentials: credentials);
                    var token = new JwtSecurityTokenHandler().WriteToken(secToken);

                    UserRole? userRole = _db.UserRoles.Where(ur => ur.UserId == user.Id).FirstOrDefault();
                    Role? roleUser = _db.Roles.Where(r => r.RoleName.ToLower() == "user").FirstOrDefault();

                    if (roleUser == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = "Role user is not found!";
                        return response;
                    }

                    if (userRole == null)
                    {
                        userRole = new()
                        {
                            UserId = user.Id,
                            RoleId = roleUser.Id
                        };

                        _db.Add(userRole);
                        _db.SaveChanges();

                        dbTran.Commit();
                    }

                    Role? role = _db.Roles.Where(r => r.Id == userRole.RoleId).FirstOrDefault();

                    if (role == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = "Role is not found!";
                        return response;
                    }

                    vAuth.Token = token;
                    vAuth.Password = string.Empty;
                    vAuth.Id = user.Id;
                    vAuth.Username = user.Username;
                    vAuth.FullName = user.FullName;
                    vAuth.Role = role.RoleName;

                    // assign response
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Login success!";
                    response.Data = vAuth;
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }


            return response;
        }
    }
}
