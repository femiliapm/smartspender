using Microsoft.EntityFrameworkCore.Storage;
using SmartSpender.DataModel;
using SmartSpender.ViewModel;
using System.Net;

namespace SmartSpender.DataAccess
{
    public class DAReminder
    {
        private readonly SmartSpenderContext _db;

        public DAReminder(SmartSpenderContext db)
        {
            _db = db;
        }

        public VMResponse<List<VMReminder>> FetchAll(VMReminderFilter? filter)
        {
            VMResponse<List<VMReminder>> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    List<Reminder> reminders = _db.Reminders.ToList();

                    reminders.ForEach(r =>
                    {
                        if (r.ReminderDate.DayOfYear - DateTime.Now.DayOfYear < 0)
                        {
                            r.IsCompleted = true;

                            _db.Update(r);
                            _db.SaveChanges();
                        }
                    });

                    dbTran.Commit();

                    List<VMReminder> data = (
                            from r in _db.Reminders
                            where (
                                filter != null && filter.IsCompleted != null ? r.IsCompleted == filter.IsCompleted : true) && (
                                filter != null && filter.DateFrom != null ? r.ReminderDate >= filter.DateFrom : true) && (
                                filter != null && filter.DateTo != null ? r.ReminderDate <= filter.DateTo : true)
                            select new VMReminder()
                            {
                                CreatedBy = r.CreatedBy,
                                CreatedOn = r.CreatedOn,
                                Id = r.Id,
                                IsCompleted = r.IsCompleted,
                                ModifiedBy = r.ModifiedBy,
                                ModifiedOn = r.ModifiedOn,
                                ReminderDate = r.ReminderDate,
                                Title = r.Title,
                                UserId = r.UserId,
                            }
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

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
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

                    _db.Add(reminder);
                    _db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = new VMReminder()
                    {
                        CreatedBy = reminder.CreatedBy,
                        CreatedOn = reminder.CreatedOn,
                        Id = reminder.Id,
                        IsCompleted = reminder.IsCompleted,
                        ModifiedBy = reminder.ModifiedBy,
                        ModifiedOn = reminder.ModifiedOn,
                        ReminderDate = reminder.ReminderDate,
                        Title = reminder.Title,
                        UserId = reminder.UserId,
                    };
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
    }
}
