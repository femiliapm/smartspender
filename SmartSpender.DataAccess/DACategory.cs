using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmartSpender.DataModel;
using SmartSpender.ViewModel;
using System.Net;

namespace SmartSpender.DataAccess
{
    public class DACategory
    {
        private readonly SmartSpenderContext _db;

        public DACategory(SmartSpenderContext db)
        {
            _db = db;
        }

        public VMResponse<List<VMCategory>> FetchAllORM()
        {
            VMResponse<List<VMCategory>> response = new();

            try
            {
                response.Data = (
                        from c in _db.Categories
                        orderby c.CreatedOn descending
                        select new VMCategory()
                        {
                            CategoryName = c.CategoryName,
                            CreatedBy = c.CreatedBy,
                            CreatedOn = c.CreatedOn,
                            Description = c.Description,
                            Id = c.Id,
                            ModifiedBy = c.ModifiedBy,
                            ModifiedOn = c.ModifiedOn,
                        }
                    ).ToList();

                response.Message = $"{response.Data.Count} data fetched!";
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        public async Task<VMResponse<List<VMCategory>>> FetchAllRaw()
        {
            VMResponse<List<VMCategory>> response = new();

            try
            {
                string sql = "SELECT * FROM categories " +
                    "ORDER BY created_on DESC";
                List<Category> dataDM = await _db.Categories.FromSqlRaw(sql).ToListAsync();

                // mapping from DM to VM
                response.Data = new();
                foreach (var data in dataDM)
                {
                    response.Data.Add(new VMCategory()
                    {
                        CategoryName = data.CategoryName,
                        CreatedBy = data.CreatedBy,
                        CreatedOn = data.CreatedOn,
                        Description = data.Description,
                        Id = data.Id,
                        ModifiedBy = data.ModifiedBy,
                        ModifiedOn = data.ModifiedOn,
                    });
                }

                response.Message = $"{response.Data.Count} data fetched!";
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        public VMResponse<VMCategory> FetchByIdORM(Guid id)
        {
            VMResponse<VMCategory> response = new();

            try
            {
                response.Data = (
                        from c in _db.Categories
                        where c.Id == id
                        orderby c.CreatedOn descending
                        select new VMCategory()
                        {
                            CategoryName = c.CategoryName,
                            CreatedBy = c.CreatedBy,
                            CreatedOn = c.CreatedOn,
                            Description = c.Description,
                            Id = c.Id,
                            ModifiedBy = c.ModifiedBy,
                            ModifiedOn = c.ModifiedOn,
                        }
                    ).FirstOrDefault();

                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successfully fetched!";
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        public async Task<VMResponse<VMCategory>> FetchByIdRaw(Guid id)
        {
            VMResponse<VMCategory> response = new();

            try
            {
                string sqlFind = "SELECT * FROM categories " +
                    "WHERE id = {0}";
                Category? category = await _db.Categories.FromSqlRaw(
                    sqlFind,
                    id).FirstOrDefaultAsync();

                if (category == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = $"Category is not found!";
                    return response;
                }

                response.Data = new VMCategory()
                {
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    Type = category.Type,
                    CreatedBy = category.CreatedBy,
                    CreatedOn = category.CreatedOn,
                    Id = category.Id,
                    ModifiedBy = category.ModifiedBy,
                    ModifiedOn = category.ModifiedOn,
                };
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successfully fetched!";
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        public VMResponse<VMCategory> CreateORM(VMCategoryReq req)
        {
            VMResponse<VMCategory> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    //mencegah duplikasi pada category name
                    Category? category = _db.Categories.Where(c =>
                        c.CategoryName.Trim().ToLower() == req.CategoryName.Trim().ToLower()).FirstOrDefault();

                    //jika category ada isinya / tidak null
                    if (category != null)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = $"Category {category.CategoryName} is already saved!";
                        return response;
                    }

                    //jika null maka akan dibuatkan model category baru sesuai dari request
                    category = new Category
                    {
                        CategoryName = req.CategoryName.Trim(),
                        Description = req.Description,
                        Type = req.Type,
                        CreatedBy = req.ModifiedBy
                    };

                    // save to db
                    _db.Add(category);
                    _db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = new VMCategory()
                    {
                        CategoryName = category.CategoryName,
                        Description = category.Description,
                        Type = category.Type,
                        CreatedBy = category.CreatedBy,
                        CreatedOn = category.CreatedOn,
                        Id = category.Id,
                        ModifiedBy = category.ModifiedBy,
                        ModifiedOn = category.ModifiedOn,
                    };
                    response.StatusCode = HttpStatusCode.Created;
                    response.Message = $"Category {category.CategoryName} is successfully saved!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public async Task<VMResponse<VMCategory>> CreateRaw(VMCategoryReq req)
        {
            VMResponse<VMCategory> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    string sqlFind = "SELECT * FROM categories " +
                        "WHERE TRIM(LOWER(category_name)) = TRIM(LOWER({0}))";
                    Category? category = await _db.Categories.FromSqlRaw(
                        sqlFind,
                        req.CategoryName).FirstOrDefaultAsync();

                    //jika category ada isinya / tidak null
                    if (category != null)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = $"Category {category.CategoryName} is already saved!";
                        return response;
                    }

                    string sql = "INSERT INTO categories (category_name, description, [type], created_by) " +
                        "OUTPUT inserted.* " +
                        "VALUES({0}, {1}, {2}, {3})";
                    category = _db.Categories.FromSqlRaw(
                        sql,
                        req.CategoryName,
                        req.Description ?? string.Empty,
                        req.Type,
                        req.ModifiedBy ?? null!)
                        .AsEnumerable().FirstOrDefault();

                    dbTran.Commit();

                    // response
                    response.Data = new VMCategory()
                    {
                        CategoryName = category!.CategoryName,
                        Description = category.Description,
                        Type = category.Type,
                        CreatedBy = category.CreatedBy,
                        CreatedOn = category.CreatedOn,
                        Id = category.Id,
                        ModifiedBy = category.ModifiedBy,
                        ModifiedOn = category.ModifiedOn,
                    };
                    response.StatusCode = HttpStatusCode.Created;
                    response.Message = $"Category {category.CategoryName} is successfully saved!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public VMResponse<VMCategory> UpdateORM(VMCategoryReq req, Guid id)
        {
            VMResponse<VMCategory> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    //prevent duplicate category name
                    Category? category = _db.Categories.Where(c =>
                        c.CategoryName == req.CategoryName
                        && c.Id != id).FirstOrDefault();

                    if (category != null)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = $"Category {category.CategoryName} is already saved!";
                        return response;
                    }

                    category = _db.Categories.Where(c => c.Id == id).FirstOrDefault();

                    if (category == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Category is not found!";
                        return response;
                    }

                    category.CategoryName = req.CategoryName;
                    category.Description = req.Description;
                    category.Type = req.Type;
                    category.ModifiedBy = req.ModifiedBy;
                    category.ModifiedOn = DateTime.Now;

                    // save to db
                    _db.Update(category);
                    _db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = new VMCategory()
                    {
                        CategoryName = category!.CategoryName,
                        Description = category.Description,
                        Type = category.Type,
                        CreatedBy = category.CreatedBy,
                        CreatedOn = category.CreatedOn,
                        Id = category.Id,
                        ModifiedBy = category.ModifiedBy,
                        ModifiedOn = category.ModifiedOn,
                    };
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"Category {category.CategoryName} is successfully updated!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public async Task<VMResponse<VMCategory>> UpdateRaw(VMCategoryReq req, Guid id)
        {
            VMResponse<VMCategory> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    //prevent duplicate category name
                    string sqlFind = "SELECT * FROM categories " +
                        "WHERE TRIM(LOWER(category_name)) = TRIM(LOWER({0}))" +
                        "AND id <> {1}";
                    Category? category = await _db.Categories.FromSqlRaw(
                        sqlFind,
                        req.CategoryName,
                        id).FirstOrDefaultAsync();

                    if (category != null)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = $"Category {category.CategoryName} is already saved!";
                        return response;
                    }

                    sqlFind = "SELECT * FROM categories " +
                        "WHERE id = {0}";
                    category = await _db.Categories.FromSqlRaw(
                        sqlFind,
                        id).FirstOrDefaultAsync();

                    if (category == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Category is not found!";
                        return response;
                    }

                    string sql = "UPDATE categories " +
                        "SET category_name = {0}, " +
                        "description = {1}, " +
                        "type = {2}, " +
                        "modified_by = {3}, " +
                        "modified_on = {4} " +
                        "WHERE id = {5}";
                    var result = await _db.Database.ExecuteSqlRawAsync(
                        sql,
                        req.CategoryName,
                        req.Description ?? string.Empty,
                        req.Type,
                        req.ModifiedBy ?? null!,
                        DateTime.Now,
                        id);

                    dbTran.Commit();

                    // response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"{result} affected - Category {req.CategoryName} is successfully updated!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public VMResponse<VMCategory> DeleteORM(Guid id)
        {
            VMResponse<VMCategory> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    Category? category = _db.Categories.Where(c => c.Id == id).FirstOrDefault();

                    if (category == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Category is not found!";
                        return response;
                    }

                    // save to db
                    _db.Remove(category);
                    _db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"Category is successfully deleted!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public async Task<VMResponse<VMCategory>> DeleteRaw(Guid id)
        {
            VMResponse<VMCategory> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    string sqlFind = "SELECT * FROM categories " +
                        "WHERE id = {0}";
                    Category? category = await _db.Categories.FromSqlRaw(
                        sqlFind,
                        id).FirstOrDefaultAsync();

                    if (category == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Category is not found!";
                        return response;
                    }

                    string sql = "DELETE categories " +
                        "WHERE id = {0}";
                    var result = await _db.Database.ExecuteSqlRawAsync(
                        sql,
                        id);

                    dbTran.Commit();

                    // response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"{result} affected - Category is successfully deleted!";
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
