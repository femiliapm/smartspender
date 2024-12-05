using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartSpender.DataAccess.AddOns;
using SmartSpender.DataModel;
using SmartSpender.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace SmartSpender.DataAccess
{
    public class DAAuth
    {
        private readonly SmartSpenderContext _db;
        private readonly Hashing hash;
        private readonly string jwtIssuer;
        private readonly string jwtKey;

        public DAAuth(SmartSpenderContext db, IConfiguration config)
        {
            _db = db;
            hash = new();
            jwtIssuer = config["Jwt:Issuer"];
            jwtKey = config["Jwt:Key"];
        }

        public VMResponse<VMUser> RegisterORM(VMAuth vUser)
        {
            VMResponse<VMUser> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    // check unique email user
                    User? user = _db.Users.Where(u => u.Email == vUser.Email.Trim()).FirstOrDefault();

                    if (user != null)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Email is already registered!";
                        return response;
                    }

                    // Persiapkan entitas model
                    user = new User()
                    {
                        Email = vUser.Email.Trim(),
                        FullName = vUser.FullName,
                        PasswordHash = hash.HashPassword(vUser.Password), // proses enkripsi password
                        Username = vUser.Username ?? string.Empty
                    };

                    // save to db
                    _db.Add(user);
                    _db.SaveChanges();

                    //if (vUser.Role == null) vUser.Role = "User";
                    vUser.Role ??= "User"; // other implementation of code above


                    // find role user
                    Role? role = _db.Roles.Where(r =>
                        r.RoleName.ToLower() == vUser.Role.ToLower())
                        .FirstOrDefault();

                    if (role == null && (
                        vUser.Role.ToLower() == "user"
                        || vUser.Role.ToLower() == "admin"))
                    {
                        role = new();
                        role.RoleName = vUser.Role;

                        _db.Add(role);
                        _db.SaveChanges();
                    }
                    else
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
                    response.Data = new VMUser()
                    {
                        CreatedOn = user.CreatedOn,
                        Email = user.Email,
                        FullName = user.FullName,
                        Id = user.Id,
                        ModifiedBy = user.ModifiedBy,
                        ModifiedOn = user.ModifiedOn,
                        PasswordHash = string.Empty,
                        Username = user.Username,
                    };
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

        public async Task<VMResponse<VMUser>> RegisterRaw(VMAuth vUser)
        {
            VMResponse<VMUser> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    // check unique email user
                    string sql = "SELECT * FROM users " +
                        "WHERE email = {0}";
                    User? user = await _db.Users.FromSqlRaw(
                        sql,
                        vUser.Email.Trim()).FirstOrDefaultAsync();

                    if (user != null)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Email is already registered!";
                        return response;
                    }

                    sql = "INSERT INTO users (email, full_name, password_hash, username) " +
                        "OUTPUT inserted.* " +
                        "VALUES ({0}, {1}, {2}, {3})";
                    user = _db.Users.FromSqlRaw(sql,
                        vUser.Email,
                        vUser.FullName ?? string.Empty,
                        hash.HashPassword(vUser.Password),
                        vUser.Username ?? string.Empty)
                        .AsEnumerable().FirstOrDefault();

                    //if (vUser.Role == null) vUser.Role = "User";
                    vUser.Role ??= "User"; // other implementation of code above

                    // find role user
                    sql = "SELECT * FROM roles " +
                        "WHERE TRIM(LOWER(role_name)) = {0}";
                    Role? role = await _db.Roles.FromSqlRaw(sql,
                        vUser.Role.Trim().ToLower()).FirstOrDefaultAsync();

                    if (role == null &&
                        (vUser.Role.Trim().ToLower() == "user"
                        || vUser.Role.Trim().ToLower() == "admin"))
                    {
                        sql = "INSERT INTO roles (role_name) " +
                            "OUTPUT inserted.* " +
                            "VALUES ({0})";
                        role = _db.Roles.FromSqlRaw(sql,
                            vUser.Role.Trim())
                            .AsEnumerable().FirstOrDefault();
                    }
                    else if (role == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Role {vUser.Role} is not found!";
                        return response;
                    }

                    sql = "INSERT INTO user_roles (user_id, role_id) " +
                        "OUTPUT inserted.* " +
                        "VALUES ({0}, {1})";
                    UserRole? userRole = _db.UserRoles.FromSqlRaw(sql,
                        user!.Id,
                        role!.Id).AsEnumerable()
                        .FirstOrDefault();

                    // commit transaction
                    dbTran.Commit();

                    // response assign value
                    response.Data = new VMUser()
                    {
                        CreatedOn = user.CreatedOn,
                        Email = user.Email,
                        FullName = user.FullName,
                        Id = user.Id,
                        ModifiedBy = user.ModifiedBy,
                        ModifiedOn = user.ModifiedOn,
                        PasswordHash = string.Empty,
                        Username = user.Username,
                    };
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

        public VMResponse<VMAuth> LoginORM(VMAuth vAuth)
        {
            VMResponse<VMAuth> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    // check apakah email terdaftar atau tidak
                    User? user = _db.Users.Where(u => u.Email == vAuth.Email).FirstOrDefault();

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
                    var secToken = new JwtSecurityToken(
                        jwtIssuer,
                        jwtIssuer,
                        null,
                        expires: DateTime.Now.AddMinutes(120),
                        signingCredentials: credentials);
                    var token = new JwtSecurityTokenHandler().WriteToken(secToken);

                    // mendapatkan role user
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

        public async Task<VMResponse<VMAuth>> LoginRaw(VMAuth vAuth)
        {
            VMResponse<VMAuth> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    // check apakah email terdaftar atau tidak
                    string sql = "SELECT * FROM users " +
                        "WHERE TRIM(email) = {0}";
                    User? user = await _db.Users.FromSqlRaw(
                        sql,
                        vAuth.Email).FirstOrDefaultAsync();

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

                    // mendapatkan role user
                    sql = "SELECT * FROM user_roles " +
                        "WHERE user_id = {0}";
                    UserRole? userRole = await _db.UserRoles.FromSqlRaw(
                        sql,
                        user.Id).FirstOrDefaultAsync();

                    sql = "SELECT * FROM roles " +
                        "WHERE LOWER(role_name) = {0}";
                    Role? roleUser = await _db.Roles.FromSqlRaw(
                        sql,
                        "user").FirstOrDefaultAsync();

                    if (roleUser == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = "Role user is not found!";
                        return response;
                    }

                    if (userRole == null)
                    {
                        userRole = _db.UserRoles.FromSqlRaw(
                            "INSERT INTO user_roles (user_id, role_id) " +
                            "OUTPUT INSERTED.* " +
                            "VALUES ({0}, {1})",
                            user.Id,
                            roleUser.Id).AsEnumerable().FirstOrDefault();

                        dbTran.Commit();
                    }

                    Role? role = await _db.Roles.FromSqlRaw(
                        "SELECT * FROM roles " +
                        "WHERE id = {0}",
                        userRole!.RoleId!).FirstOrDefaultAsync();

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
