using Microsoft.EntityFrameworkCore.Storage;
using PlannerTracker.DataModel;
using PlannerTracker.ViewModel;
using System.Net;

namespace PlannerTracker.DataAccess
{
    public class DAReminder
    {
        private readonly PlannerTrackerContext db;

        public DAReminder(PlannerTrackerContext _db)
        {
            this.db = _db;
        }

        public VMResponse<List<VMReminder>> FetchAll(VMReminderFilter? filter)
        {
            VMResponse<List<VMReminder>> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    List<Reminder> reminders = db.Reminders.ToList();

                    reminders.ForEach(r =>
                    {
                        if (r.ReminderDate.DayOfYear - DateTime.Now.DayOfYear < 0)
                        {
                            r.IsCompleted = true;

                            db.Update(r);
                            db.SaveChanges();
                        }
                    });

                    dbTran.Commit();

                    List<VMReminder> data = (
                            from r in db.Reminders
                            where r.IsDelete == false && (
                                filter != null && filter.IsCompleted != null ? r.IsCompleted == filter.IsCompleted : true) && (
                                filter != null && filter.DateFrom != null ? r.ReminderDate >= filter.DateFrom : true) && (
                                filter != null && filter.DateTo != null ? r.ReminderDate <= filter.DateTo : true)
                            select new VMReminder(r)
                        ).ToList();

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

        public VMResponse<VMReminder> Create(VMReminderReq req)
        {
            VMResponse<VMReminder> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    Reminder reminder = new()
                    {
                        CreatedBy = req.ModifiedBy,
                        ReminderDate = req.ReminderDate,
                        Title = req.Title,
                        UserId = req.UserId,
                        IsCompleted = (req.ReminderDate.DayOfYear - DateTime.Now.DayOfYear < 0) ? true : false,
                    };

                    db.Add(reminder);
                    db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = new VMReminder(reminder);
                    response.StatusCode = HttpStatusCode.Created;
                    response.Message = $"Reminder is successfully created!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }

                return response;
            }
        }
    }
}
