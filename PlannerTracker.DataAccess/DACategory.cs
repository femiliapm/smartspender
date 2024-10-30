using Microsoft.EntityFrameworkCore.Storage;
using PlannerTracker.DataModel;
using PlannerTracker.ViewModel;
using System.Net;

namespace PlannerTracker.DataAccess
{
    public class DACategory
    {
        private readonly PlannerTrackerContext _db;

        public DACategory(PlannerTrackerContext db)
        {
            _db = db;
        }

        public VMResponse<VMCategory> Create(VMCategoryReq req)
        {
            VMResponse<VMCategory> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    //prevent duplicate category name
                    Category? category = _db.Categories.Where(c => c.CategoryName == req.CategoryName && c.IsDelete == false).FirstOrDefault();

                    if (category != null)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = $"Category {category.CategoryName} is already saved!";
                        return response;
                    }

                    category = new Category
                    {
                        CategoryName = req.CategoryName,
                        Description = req.Description,
                        Type = req.Type,
                        CreatedBy = req.CreatedBy
                    };

                    // save to db
                    _db.Add(category);
                    _db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = new VMCategory(category);
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

        public VMResponse<VMCategory> Update(VMCategoryReq req, Guid id)
        {
            VMResponse<VMCategory> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    //prevent duplicate category name
                    Category? category = _db.Categories.Where(c => c.CategoryName == req.CategoryName && c.Id != id && c.IsDelete == false).FirstOrDefault();

                    if (category != null)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = $"Category {category.CategoryName} is already saved!";
                        return response;
                    }

                    category = _db.Categories.Where(c => c.Id == id && c.IsDelete == false).FirstOrDefault();

                    if (category == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Category is not found!";
                        return response;
                    }

                    category.CategoryName = req.CategoryName;
                    category.Description = req.Description;
                    category.Type = req.Type;
                    category.ModifiedBy = req.CreatedBy;
                    category.ModifiedOn = DateTime.Now;

                    // save to db
                    _db.Update(category);
                    _db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = new VMCategory(category);
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

        public VMResponse<List<VMCategory>> GetCategories(string filter)
        {
            VMResponse<List<VMCategory>> response = new();

            try
            {
                response.Data = (
                        from c in _db.Categories
                        where c.IsDelete == false && (
                            !string.IsNullOrEmpty(filter)
                                ? c.CategoryName.ToLower().Contains(filter.ToLower()) || c.Type.ToLower().Contains(filter.ToLower()) || c.Description!.ToLower().Contains(filter.ToLower())
                                : true
                        )
                        orderby c.CreatedOn descending
                        select new VMCategory(c)
                    ).ToList();

                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successfully fetched!";
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        public VMResponse<VMCategory> GetCategoryById(Guid id)
        {
            VMResponse<VMCategory> response = new();

            try
            {
                response.Data = (
                        from c in _db.Categories
                        where c.IsDelete == false && c.Id == id
                        orderby c.CreatedOn descending
                        select new VMCategory(c)
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

        public VMResponse<VMCategory> DeleteById(Guid id, Guid userId)
        {
            VMResponse<VMCategory> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    Category? category = _db.Categories.Where(c => c.Id == id && c.IsDelete == false).FirstOrDefault();

                    if (category == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Category is not found!";
                        return response;
                    }

                    category.IsDelete = true;
                    category.DeletedBy = userId;
                    category.DeletedOn = DateTime.Now;

                    // save to db
                    _db.Update(category);
                    _db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"Category {category.CategoryName} is successfully deleted!";
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