using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmartSpender.DataAccess.AddOns;
using SmartSpender.DataModel;
using SmartSpender.ViewModel;
using System.Net;

namespace SmartSpender.DataAccess
{
    public class DAAuth
    {
        private readonly SmartSpenderContext _db;
        private readonly Hashing hash;

        public DAAuth(SmartSpenderContext db)
        {
            _db = db;
            hash = new();
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
                    else
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
    }
}
