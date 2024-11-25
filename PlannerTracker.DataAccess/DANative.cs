using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PlannerTracker.DataModel;
using PlannerTracker.ViewModel;
using System.Net;

namespace PlannerTracker.DataAccess
{
    public class DANative
    {
        private readonly PlannerTrackerContext db;

        public DANative(PlannerTrackerContext _db)
        {
            this.db = _db;
        }

        public async Task<VMResponse<List<Reminder>>> FetchAll()
        {
            VMResponse<List<Reminder>> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    string sql = "SELECT * FROM reminders";
                    List<Reminder> data = await db.Reminders.FromSqlRaw(sql).ToListAsync();

                    dbTran.Commit();

                    response.Data = data;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Successfully fetched!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public async Task<VMResponse<int>> Create(VMReminderReq req)
        {
            VMResponse<int> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    string sql = "INSERT INTO reminders (created_by, reminder_date, title, user_id, is_completed) VALUES ({0}, {1}, {2}, {3}, {4})";
                    var result = await db.Database.ExecuteSqlRawAsync(
                        sql,
                        req.ModifiedBy!,
                        req.ReminderDate,
                        req.Title,
                        req.UserId,
                        (req.ReminderDate.DayOfYear - DateTime.Now.DayOfYear < 0) ? true : false);

                    dbTran.Commit();

                    // response
                    response.Data = result;
                    response.StatusCode = HttpStatusCode.Created;
                    response.Message = $"Reminder is successfully created!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public async Task<VMResponse<int>> Update(Guid id, VMReminderReq req)
        {
            VMResponse<int> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    string sql = "UPDATE reminders " +
                        "SET reminder_date = {0}, " +
                        "title = {1}, " +
                        "user_id = {2}, " +
                        "modified_by = {3}, " +
                        "modified_on = {4}, " +
                        "is_completed = {5} " +
                        "WHERE id = {6}";
                    var result = await db.Database.ExecuteSqlRawAsync(
                        sql,
                        req.ReminderDate,
                        req.Title,
                        req.UserId,
                        req.ModifiedBy!,
                        DateTime.Now,
                        (req.ReminderDate.DayOfYear - DateTime.Now.DayOfYear < 0) ? true : false,
                        id);

                    dbTran.Commit();

                    // response
                    response.Data = result;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"Reminder is successfully updated!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }
            //btw ternnyata beda yg fromsql ama executesql tadi ini.. 
            return response;
        }

        public async Task<VMResponse<int>> Delete(Guid id)
        {
            VMResponse<int> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    string sql = "DELETE reminders " +
                        "WHERE id = {0}";
                    var result = await db.Database.ExecuteSqlRawAsync(sql, id);

                    dbTran.Commit();

                    // response
                    response.Data = result;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"Reminder is successfully deleted!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }
            //btw ternnyata beda yg fromsql ama executesql tadi ini.. 
            return response;
        }

        public async Task<VMResponse<int>> SoftDelete(Guid id, Guid userId)
        {
            VMResponse<int> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    string sql = "UPDATE reminders " +
                        "SET is_delete = {0}, " +
                        "deleted_by = {1}, " +
                        "deleted_on = {2}" +
                        "WHERE id = {3}";
                    var result = await db.Database.ExecuteSqlRawAsync(sql, true, userId, DateTime.Now, id);

                    dbTran.Commit();

                    // response
                    response.Data = result;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"Reminder is successfully deleted!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }
            //btw ternnyata beda yg fromsql ama executesql tadi ini.. 
            return response;
        }
    }
}
