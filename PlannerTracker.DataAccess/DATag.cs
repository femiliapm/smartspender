using Microsoft.EntityFrameworkCore.Storage;
using PlannerTracker.DataModel;
using PlannerTracker.ViewModel;
using System.Net;

namespace PlannerTracker.DataAccess
{
    public class DATag
    {
        private readonly PlannerTrackerContext db;

        public DATag(PlannerTrackerContext db)
        {
            this.db = db;
        }

        public VMResponse<List<VMTag>> GetAll()
        {
            VMResponse<List<VMTag>> response = new();

            try
            {
                response.Data = (
                        from t in db.Tags
                        where t.IsDelete == false
                        select new VMTag(t)
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

        public VMResponse<List<string>> GetAllTagName()
        {
            VMResponse<List<string>> response = new();

            try
            {
                response.Data = (
                        from t in db.Tags
                        where t.IsDelete == false
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

        public VMResponse<VMTag> Create(VMTagReq req)
        {
            VMResponse<VMTag> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    Tag tag = new()
                    {
                        CreatedBy = req.ModifiedBy,
                        TagName = req.TagName
                    };

                    db.Add(tag);
                    db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = new VMTag(tag);
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
    }
}
