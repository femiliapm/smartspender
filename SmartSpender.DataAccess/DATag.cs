using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmartSpender.DataModel;
using SmartSpender.ViewModel;
using System.Net;

namespace SmartSpender.DataAccess
{
    public class DATag
    {
        private readonly SmartSpenderContext _db;

        public DATag(SmartSpenderContext db)
        {
            _db = db;
        }

        public VMResponse<List<VMTag>> FetchAllORM()
        {
            VMResponse<List<VMTag>> response = new();

            try
            {
                response.Data = (
                        from t in _db.Tags
                        select new VMTag()
                        {
                            CreatedBy = t.CreatedBy,
                            CreatedOn = t.CreatedOn,
                            Id = t.Id,
                            ModifiedBy = t.ModifiedBy,
                            ModifiedOn = t.ModifiedOn,
                            TagName = t.TagName,
                        }
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

        public async Task<VMResponse<List<VMTag>>> FetchAllRaw()
        {
            VMResponse<List<VMTag>> response = new();

            try
            {
                List<Tag> tags = await _db.Tags
                    .FromSqlRaw("SELECT * FROM tags " +
                    "ORDER BY created_on DESC").ToListAsync();

                response.Data = tags.Select(t =>
                {
                    return new VMTag()
                    {
                        CreatedBy = t.CreatedBy,
                        CreatedOn = t.CreatedOn,
                        Id = t.Id,
                        ModifiedBy = t.ModifiedBy,
                        ModifiedOn = t.ModifiedOn,
                        TagName = t.TagName,
                    };
                }).ToList();

                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successfully fetched!";
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        public VMResponse<List<string>> FetchAllTagNameORM()
        {
            VMResponse<List<string>> response = new();

            try
            {
                response.Data = (
                        from t in _db.Tags
                        select t.TagName
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

        public async Task<VMResponse<List<string>>> FetchAllTagNameRaw()
        {
            VMResponse<List<string>> response = new();

            try
            {
                List<Tag> tags = await _db.Tags
                    .FromSqlRaw("SELECT * FROM tags " +
                    "ORDER BY created_on DESC").ToListAsync();

                response.Data = tags.Select(t => t.TagName).ToList();
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successfully fetched!";
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        public VMResponse<VMTag> CreateORM(VMTagReq req)
        {
            VMResponse<VMTag> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    Tag tag = new()
                    {
                        CreatedBy = req.ModifiedBy,
                        TagName = req.TagName
                    };

                    _db.Add(tag);
                    _db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = new VMTag()
                    {
                        CreatedBy = tag.CreatedBy,
                        CreatedOn = tag.CreatedOn,
                        Id = tag.Id,
                        ModifiedBy = tag.ModifiedBy,
                        ModifiedOn = tag.ModifiedOn,
                        TagName = tag.TagName,
                    };
                    response.StatusCode = HttpStatusCode.Created;
                    response.Message = $"Tag {tag.TagName} is successfully created!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public async Task<VMResponse<VMTag>> CreateRaw(VMTagReq req)
        {
            VMResponse<VMTag> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    var result = await _db.Database
                        .ExecuteSqlRawAsync(
                            "INSERT INTO tags (created_by, tag_name) " +
                            "VALUES ({0}, {1})",
                            req.ModifiedBy!,
                            req.TagName);

                    dbTran.Commit();

                    // response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.Created;
                    response.Message = $"Tag {req.TagName} is successfully created!";
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
